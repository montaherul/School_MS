/* ═══════════════════════════════════════════════════════════════════
   SchoolMS — Enterprise Grid Framework
   Shared Tabulator wrapper for ALL modules
   ═══════════════════════════════════════════════════════════════════ */
(function () {
    'use strict';

    window.SchoolMS = window.SchoolMS || {};
    if (window.SchoolMS.Grid) return;
    window.SchoolMS.Grid = {};

    var _instances = {};

    function getToken() {
        var el = document.querySelector('input[name="__RequestVerificationToken"]');
        return el ? el.value : '';
    }

    function authHeaders() {
        var h = { 'Content-Type': 'application/json', 'X-Requested-With': 'XMLHttpRequest' };
        var t = getToken();
        if (t) h['RequestVerificationToken'] = t;
        return h;
    }

    function toast(type, msg) {
        if (window.toastr && toastr[type]) { toastr[type](msg); return; }
        if (window.showExamToast) { showExamToast(msg, type); return; }
    }

    function buildColumns(conf) {
        var cols = [];
        if (conf.rowNum !== false) {
            cols.push({ title: '#', formatter: 'rownum', hozAlign: 'center', width: 50, headerSort: false, frozen: true });
        }
        if (conf.selectable) {
            cols.push({ formatter: 'rowSelection', titleFormatter: 'rowSelection', hozAlign: 'center', width: 50, headerSort: false, frozen: true });
        }
        if (conf.columns) {
            conf.columns.forEach(function (c) { cols.push(c); });
        }
        if (conf.actions !== false) {
            cols.push({
                title: 'Actions', field: 'id', hozAlign: 'center', headerSort: false,
                width: conf.actionsWidth || 160,
                frozen: true,
                formatter: function (c) {
                    var d = c.getRow().getData();
                    var items = conf.actionItems ? conf.actionItems(d) : SchoolMS.Grid._defaultActions(d, conf);
                    return window.buildActionGroup ? buildActionGroup(items) : '';
                }
            });
        }
        return cols;
    }

    SchoolMS.Grid._defaultActions = function (d, conf) {
        var items = [];
        var editUrl = conf.editUrl ? (typeof conf.editUrl === 'function' ? conf.editUrl(d) : conf.editUrl + '/' + d.id) : null;
        if (editUrl) items.push({ url: editUrl, className: 'action-btn--edit', icon: 'pencil', title: 'Edit' });
        if (conf.deleteUrl) {
            items.push({ className: 'action-btn--delete', icon: 'trash', title: 'Delete', onClick: 'SchoolMS.Grid.delete(\'' + conf.name + '\',' + d.id + ')' });
        }
        return items;
    };

    var _deleteMeta = { name: null, id: null };

    SchoolMS.Grid.delete = function (name, id) {
        _deleteMeta.name = name;
        _deleteMeta.id = id;
        var el = document.getElementById('gridDeleteModal');
        if (el) { el.classList.remove('ws-d-none'); return; }
    };

    function closeDeleteModal() {
        var el = document.getElementById('gridDeleteModal');
        if (el) el.classList.add('ws-d-none');
        _deleteMeta.name = null; _deleteMeta.id = null;
    }

    SchoolMS.Grid.confirmDelete = function () {
        var meta = _deleteMeta;
        if (!meta.name || !meta.id) { closeDeleteModal(); return; }
        var inst = _instances[meta.name];
        if (!inst || !inst.config.deleteUrl) { closeDeleteModal(); toast('error', 'No delete URL'); return; }
        var url = typeof inst.config.deleteUrl === 'function'
            ? inst.config.deleteUrl(meta.id)
            : inst.config.deleteUrl + '/' + meta.id;
        var method = inst.config.deleteMethod || 'POST';
        var opts = { method: method, headers: authHeaders() };
        if (inst.config.deleteBody) {
            opts.headers['Content-Type'] = 'application/json';
            opts.body = JSON.stringify(typeof inst.config.deleteBody === 'object' ? inst.config.deleteBody(meta.id) : { id: meta.id });
        }
        fetch(url, opts)
            .then(function (r) {
                var ct = r.headers.get('content-type') || '';
                if (ct.indexOf('application/json') !== -1) return r.json().then(function (d) { d._json = true; return d; });
                return { _json: false, success: true };
            })
            .then(function (res) {
                closeDeleteModal();
                if (res._json && res.success) { toast('success', res.message || 'Deleted'); if (inst) inst.table.replaceData(); }
                else if (res._json) { toast('error', res.message || 'Delete failed'); }
                else { if (inst) { inst.table.replaceData(); } toast('success', 'Deleted'); }
            })
            .catch(function () { closeDeleteModal(); toast('error', 'Network error'); });
    };

    SchoolMS.Grid.cancelDelete = function () { closeDeleteModal(); };

    function buildToolbar(container, conf) {
        var html = '<div class="adm-filters ws-mb-3" id="grid-toolbar-' + conf.name + '"><div class="adm-filters__row">';
        if (conf.search !== false) {
            html += '<div class="adm-filters__field adm-filters__field--search"><div class="adm-input-wrap">';
            html += '<i class="bi bi-search adm-input-wrap__icon"></i>';
            html += '<input type="text" class="adm-input grid-search-input" placeholder="Search..." data-grid="' + conf.name + '">';
            html += '</div></div>';
        }
        if (conf.filters) {
            conf.filters.forEach(function (f) {
                html += '<div class="adm-filters__field">';
                if (f.label) html += '<label class="adm-label">' + f.label + '</label>';
                if (f.type === 'select') {
                    html += '<select class="adm-select grid-filter" data-grid="' + conf.name + '" data-field="' + f.field + '">';
                    html += '<option value="">' + (f.placeholder || 'All') + '</option>';
                    (f.options || []).forEach(function (o) { html += '<option value="' + o.value + '">' + o.label + '</option>'; });
                    html += '</select>';
                } else {
                    html += '<input type="' + (f.type || 'text') + '" class="adm-input grid-filter" data-grid="' + conf.name + '" data-field="' + f.field + '" placeholder="' + (f.placeholder || '') + '">';
                }
                html += '</div>';
            });
        }
        html += '<div class="adm-filters__field adm-filters__field--actions">';
        if (conf.refresh !== false) {
            html += '<button class="adm-btn adm-btn--ghost grid-btn-refresh" data-grid="' + conf.name + '" title="Refresh"><i class="bi bi-arrow-clockwise"></i></button>';
        }
        if (conf.resetFilters !== false) {
            html += '<button class="adm-btn adm-btn--ghost grid-btn-reset" data-grid="' + conf.name + '" title="Reset"><i class="bi bi-x-circle"></i></button>';
        }
        if (conf.exportBtn !== false) {
            html += '<div class="ws-dropdown ws-dropdown--up">';
            html += '<button class="adm-btn adm-btn--ghost" data-toggle="ws-dropdown" title="Export"><i class="bi bi-download"></i></button>';
            html += '<div class="ws-dropdown__menu ws-d-none">';
            html += '<button class="ws-dropdown__item grid-btn-export" data-grid="' + conf.name + '" data-type="csv"><i class="bi bi-filetype-csv ws-me-2"></i>CSV</button>';
            html += '<button class="ws-dropdown__item grid-btn-export" data-grid="' + conf.name + '" data-type="xlsx"><i class="bi bi-file-earmark-excel ws-me-2"></i>Excel</button>';
            html += '<button class="ws-dropdown__item grid-btn-export" data-grid="' + conf.name + '" data-type="pdf"><i class="bi bi-file-earmark-pdf ws-me-2"></i>PDF</button>';
            html += '<div class="ws-dropdown__divider"></div>';
            html += '<button class="ws-dropdown__item grid-btn-print" data-grid="' + conf.name + '"><i class="bi bi-printer ws-me-2"></i>Print</button>';
            html += '<button class="ws-dropdown__item grid-btn-copy" data-grid="' + conf.name + '"><i class="bi bi-clipboard ws-me-2"></i>Copy</button>';
            html += '</div></div>';
        }
        if (conf.density !== false) {
            html += '<div class="ws-dropdown ws-dropdown--up">';
            html += '<button class="adm-btn adm-btn--ghost" data-toggle="ws-dropdown" title="Density"><i class="bi bi-view-list"></i></button>';
            html += '<div class="ws-dropdown__menu ws-d-none">';
            html += '<button class="ws-dropdown__item grid-btn-density" data-grid="' + conf.name + '" data-density="comfortable"><i class="bi bi-list ws-me-2"></i>Comfortable</button>';
            html += '<button class="ws-dropdown__item grid-btn-density" data-grid="' + conf.name + '" data-density="compact"><i class="bi bi-list-nested ws-me-2"></i>Compact</button>';
            html += '<button class="ws-dropdown__item grid-btn-density" data-grid="' + conf.name + '" data-density="minimal"><i class="bi bi-justify ws-me-2"></i>Minimal</button>';
            html += '</div></div>';
        }
        if (conf.addUrl) {
            html += '<a href="' + (typeof conf.addUrl === 'function' ? conf.addUrl() : conf.addUrl) + '" class="adm-btn adm-btn--primary"><i class="bi bi-plus-circle ws-me-1"></i>Add New</a>';
        }
        html += '</div></div></div>';
        container.insertAdjacentHTML('afterbegin', html);
        wireToolbar(conf.name);
    }

    function wireToolbar(name) {
        var inst = _instances[name];
        if (!inst) return;
        var prefix = '#grid-toolbar-' + name;
        var searchEl = document.querySelector(prefix + ' .grid-search-input');
        if (searchEl) {
            var searchTimer;
            searchEl.addEventListener('input', function () {
                clearTimeout(searchTimer);
                searchTimer = setTimeout(function () {
                    if (inst.config.searchFn) { inst.config.searchFn(searchEl.value); }
                    else {
                        var s = searchEl.value;
                        if (inst.table) {
                            if (s) inst.table.setFilter('global', 'like', s);
                            else inst.table.clearFilter(true);
                        }
                    }
                }, 300);
            });
        }
        document.querySelectorAll(prefix + ' .grid-filter').forEach(function (el) {
            el.addEventListener('change', function () {
                var field = el.getAttribute('data-field');
                var val = el.value;
                if (inst.config.filterFn) { inst.config.filterFn(field, val); }
                else if (inst.table) {
                    if (val) inst.table.setFilter(field, 'like', val);
                    else inst.table.removeFilter(field);
                }
            });
        });
        var r = document.querySelector(prefix + ' .grid-btn-refresh');
        if (r) r.addEventListener('click', function () { SchoolMS.Grid.reload(name); });
        var rs = document.querySelector(prefix + ' .grid-btn-reset');
        if (rs) rs.addEventListener('click', function () {
            document.querySelectorAll(prefix + ' .grid-filter, ' + prefix + ' .grid-search-input').forEach(function (e) { e.value = ''; });
            SchoolMS.Grid.resetFilters(name);
        });
        document.querySelectorAll(prefix + ' .grid-btn-export').forEach(function (b) {
            b.addEventListener('click', function () { SchoolMS.Grid.download(name, b.getAttribute('data-type')); });
        });
        var pb = document.querySelector(prefix + ' .grid-btn-print');
        if (pb) pb.addEventListener('click', function () { SchoolMS.Grid.print(name); });
        var cb = document.querySelector(prefix + ' .grid-btn-copy');
        if (cb) cb.addEventListener('click', function () { SchoolMS.Grid.copyToClipboard(name); });
        document.querySelectorAll(prefix + ' .grid-btn-density').forEach(function (b) {
            b.addEventListener('click', function () { SchoolMS.Grid.setDensity(name, b.getAttribute('data-density')); });
        });
        document.querySelectorAll(prefix + ' [data-toggle="ws-dropdown"]').forEach(function (b) {
            b.addEventListener('click', function (e) {
                e.stopPropagation();
                var m = b.nextElementSibling;
                if (m) m.classList.toggle('ws-d-none');
            });
        });
        document.addEventListener('click', function () {
            document.querySelectorAll(prefix + ' .ws-dropdown__menu').forEach(function (m) { m.classList.add('ws-d-none'); });
        });
    }

    /* ═══ PUBLIC API ════════════════════════════════════════════ */
    SchoolMS.Grid.create = function (name, config) {
        if (_instances[name]) SchoolMS.Grid.destroy(name);
        config = config || {};
        config.name = name;
        var container = document.querySelector(config.container || '#data-table');
        if (!container) { console.error('Grid: container not found', config.container); return; }
        var columns = buildColumns(config);
        var table = new Tabulator(container, {
            height: config.height || false,
            layout: config.layout || 'fitColumns',
            columns: columns,
            movableColumns: config.movableColumns !== false,
            resizableColumns: config.resizableColumns !== false,
            selectable: config.selectable || false,
            selectableRangeMode: 'click',
            responsiveLayout: config.responsiveLayout || 'collapse',
            placeholder: config.placeholder || '<div class="dash-empty"><i class="bi bi-inbox ws-fs-48"></i><div class="dash-empty__title">No Records Found</div></div>',
            pagination: config.pagination !== false ? 'remote' : false,
            paginationSize: config.pageSize || 20,
            paginationSizeSelector: config.pageSizeSelector || [10, 20, 50, 100],
            paginationMode: 'remote',
            paginationDataSent: config.paginationParams || { page: 'page', size: 'size' },
            filterMode: 'remote',
            ajaxURL: config.ajaxUrl,
            ajaxParams: config.ajaxParams || {},
            ajaxConfig: config.ajaxConfig || { headers: { 'X-Requested-With': 'XMLHttpRequest' } },
            ajaxResponse: function (url, params, response) {
                var data = response.data || response.rows || response;
                var lastPage = response.last_page || response.totalPages || 1;
                return { data: data, last_page: lastPage };
            },
            persistenceMode: 'cookie',
            dataLoading: function () { SchoolMS.Grid.showLoader(name); },
            dataLoaded: function () { SchoolMS.Grid.hideLoader(name); },
            dataLoadError: function (err) { SchoolMS.Grid.hideLoader(name); SchoolMS.Grid.showError(name, err.message || 'Server error'); }
        });
        var inst = { table: table, config: config };
        _instances[name] = inst;
        if (config.toolbar !== false) buildToolbar(container.parentNode || container, config);
        return table;
    };

    SchoolMS.Grid.destroy = function (name) {
        var inst = _instances[name];
        if (inst && inst.table) { try { inst.table.destroy(); } catch (e) {} }
        delete _instances[name];
    };

    SchoolMS.Grid.reload = function (name) {
        var inst = _instances[name];
        if (inst && inst.table) inst.table.replaceData();
    };

    SchoolMS.Grid.download = function (name, type) {
        var inst = _instances[name];
        if (!inst || !inst.table) return;
        var opts = { type: type };
        if (type === 'pdf') opts.orientation = 'landscape';
        inst.table.download(type, name + '-export.' + type, opts);
    };

    SchoolMS.Grid.print = function (name) {
        var inst = _instances[name];
        if (inst && inst.table) inst.table.print(false, true);
    };

    SchoolMS.Grid.copyToClipboard = function (name) {
        var inst = _instances[name];
        if (inst && inst.table) inst.table.copyToClipboard('all');
    };

    SchoolMS.Grid.getSelected = function (name) {
        var inst = _instances[name];
        return inst && inst.table ? inst.table.getSelectedData() : [];
    };

    SchoolMS.Grid.clearSelection = function (name) {
        var inst = _instances[name];
        if (inst && inst.table) inst.table.deselectRow();
    };

    SchoolMS.Grid.resetFilters = function (name) {
        var inst = _instances[name];
        if (inst && inst.table) { inst.table.clearFilter(true); inst.table.clearSort(); }
    };

    SchoolMS.Grid.setFilters = function (name, filters) {
        var inst = _instances[name];
        if (inst && inst.table) {
            inst.table.clearFilter(true);
            Object.keys(filters).forEach(function (f) { if (filters[f]) inst.table.setFilter(f, 'like', filters[f]); });
        }
    };

    SchoolMS.Grid.setDensity = function (name, density) {
        var inst = _instances[name];
        if (inst && inst.table) inst.table.setStyle(density);
        try { localStorage.setItem('sms_grid_density_' + name, density); } catch (e) {}
    };

    SchoolMS.Grid.showLoader = function (name) {
        var el = document.getElementById('grid-toolbar-' + name);
        if (el) { var l = el.querySelector('.grid-loader'); if (!l) { var d = document.createElement('div'); d.className = 'grid-loader'; d.innerHTML = '<div class="adm-loading adm-loading--visible"><div class="adm-loading__spinner"></div></div>'; el.parentNode.insertBefore(d, el.nextSibling); } }
    };

    SchoolMS.Grid.hideLoader = function (name) {
        var el = document.getElementById('grid-toolbar-' + name);
        if (el) { var l = el.parentNode.querySelector('.grid-loader'); if (l) l.remove(); }
    };

    SchoolMS.Grid.showError = function (name, msg) {
        var inst = _instances[name];
        if (inst && inst.table) {
            inst.table.clearData();
            inst.table.setPlaceholder('<div class="dash-empty"><i class="bi bi-exclamation-triangle ws-fs-48" style="color:var(--adm-danger)"></i><div class="dash-empty__title">' + (msg || 'Error loading data') + '</div></div>');
        }
    };

    SchoolMS.Grid.bulkDelete = function (name) {
        var selected = SchoolMS.Grid.getSelected(name);
        if (!selected.length) { toast('warning', 'No rows selected'); return; }
        _deleteMeta.name = name;
        _deleteMeta.id = selected.map(function (r) { return r.id; }).join(',');
        var el = document.getElementById('gridDeleteModal');
        if (el) {
            el.querySelector('.grid-delete-count').textContent = selected.length;
            el.classList.remove('ws-d-none');
        }
    };
})();
