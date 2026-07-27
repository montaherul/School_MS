(function () {
    'use strict';

    // ── Floating Labels ────────────────────────
    function initFloatingLabels(root) {
        var groups = (root || document).querySelectorAll('.sms-floating-group');
        groups.forEach(function (group) {
            var input = group.querySelector('.sms-floating-input, .sms-floating-select');
            if (!input) return;
            var check = function () {
                var hasValue = !!input.value;
                if (input.tagName === 'SELECT') {
                    hasValue = input.selectedIndex > 0 && input.options[input.selectedIndex].value !== '';
                }
                input.classList.toggle('sms-floating-input--filled', hasValue);
                input.classList.toggle('sms-floating-select--filled', hasValue);
            };
            check();
            input.addEventListener('input', check);
            input.addEventListener('change', check);
            input.addEventListener('blur', check);
        });
    }

    // ── File Upload Previews ───────────────────
    function initFileUploads(root) {
        var dropzones = (root || document).querySelectorAll('.sms-file-upload__dropzone input[type="file"]');
        dropzones.forEach(function (input) {
            var container = input.closest('.sms-file-upload') || input.closest('.sms-file-upload__dropzone');
            if (!container) return;
            var previewsContainer = container.querySelector('.sms-file-upload__previews');
            if (!previewsContainer) return;

            input.addEventListener('change', function () {
                previewsContainer.innerHTML = '';
                var files = Array.from(input.files);
                files.forEach(function (file) {
                    if (!file.type.startsWith('image/')) return;
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        var el = document.createElement('div');
                        el.className = 'sms-file-upload__preview';
                        el.innerHTML = '<img src="' + e.target.result + '" alt="' + file.name + '">' +
                            '<button type="button" class="sms-file-upload__preview-remove" title="Remove">&times;</button>' +
                            '<div class="sms-file-upload__file-name">' + file.name + '</div>';
                        el.querySelector('.sms-file-upload__preview-remove').addEventListener('click', function () {
                            el.remove();
                            input.value = '';
                        });
                        previewsContainer.appendChild(el);
                    };
                    reader.readAsDataURL(file);
                });
            });

            // Drag highlight
            var dropzone = input.closest('.sms-file-upload__dropzone');
            if (dropzone) {
                ['dragenter', 'dragover'].forEach(function (evt) {
                    dropzone.addEventListener(evt, function () { dropzone.classList.add('sms-file-upload__dropzone--active'); });
                });
                ['dragleave', 'drop'].forEach(function (evt) {
                    dropzone.addEventListener(evt, function () { dropzone.classList.remove('sms-file-upload__dropzone--active'); });
                });
            }
        });
    }

    // ── Searchable Select (simple inline filter) ──
    function initSearchableSelects(root) {
        var selects = (root || document).querySelectorAll('.sms-searchable-select');
        selects.forEach(function (original) {
            if (original.dataset.smsEnhanced === 'true') return;
            original.dataset.smsEnhanced = 'true';

            var wrapper = document.createElement('div');
            wrapper.className = 'sms-searchable-select__wrapper';
            wrapper.style.position = 'relative';

            var search = document.createElement('input');
            search.type = 'text';
            search.className = 'sms-floating-input sms-searchable-select__input';
            search.placeholder = 'Search...';
            search.style.cssText = 'width:100%;padding:9px 13px;font-size:13.5px;border:1px solid var(--adm-border);border-radius:var(--adm-radius);background:var(--adm-surface);color:var(--adm-text);outline:none;font-family:inherit;';

            var dropdown = document.createElement('div');
            dropdown.className = 'sms-searchable-select__dropdown';
            dropdown.style.cssText = 'position:absolute;top:100%;left:0;right:0;z-index:200;background:var(--adm-surface);border:1px solid var(--adm-border);border-radius:var(--adm-radius);box-shadow:var(--adm-shadow-lg);max-height:200px;overflow-y:auto;display:none;margin-top:4px;';

            var items = [];
            var opts = Array.from(original.options);
            opts.forEach(function (opt, idx) {
                if (opt.value === '' && !opt.label) return;
                var item = document.createElement('div');
                item.className = 'sms-searchable-select__item';
                item.textContent = opt.label || opt.text;
                item.dataset.value = opt.value;
                item.dataset.index = idx;
                item.style.cssText = 'padding:8px 12px;font-size:13px;cursor:pointer;transition:background 0.15s;';
                item.addEventListener('mouseenter', function () { item.style.background = 'var(--adm-surface-2)'; });
                item.addEventListener('mouseleave', function () { item.style.background = ''; });
                item.addEventListener('click', function () {
                    original.value = item.dataset.value;
                    search.value = item.textContent;
                    dropdown.style.display = 'none';
                    original.dispatchEvent(new Event('change'));
                });
                dropdown.appendChild(item);
                items.push(item);
            });

            search.addEventListener('focus', function () {
                dropdown.style.display = 'block';
                filterItems('');
            });

            search.addEventListener('input', function () {
                filterItems(search.value.toLowerCase());
                dropdown.style.display = 'block';
            });

            document.addEventListener('click', function (e) {
                if (!wrapper.contains(e.target)) {
                    dropdown.style.display = 'none';
                }
            });

            original.style.display = 'none';
            if (original.selectedIndex > 0) {
                var selOpt = original.options[original.selectedIndex];
                search.value = selOpt.label || selOpt.text;
            }

            wrapper.appendChild(search);
            wrapper.appendChild(dropdown);
            original.parentNode.insertBefore(wrapper, original);
            wrapper.appendChild(original);

            function filterItems(query) {
                var first = true;
                items.forEach(function (item) {
                    var match = item.textContent.toLowerCase().indexOf(query) !== -1;
                    item.style.display = match ? 'block' : 'none';
                    if (match && first) {
                        item.style.background = 'var(--adm-primary-bg)';
                        first = false;
                    } else {
                        item.style.background = '';
                    }
                });
            }
        });
    }

    // ── Init on DOM ready ─────────────────────
    function init() {
        initFloatingLabels();
        initFileUploads();
        initSearchableSelects();
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

    // Expose for dynamic content (e.g. after AJAX load)
    window.schoolmsForms = {
        initFloatingLabels: initFloatingLabels,
        initFileUploads: initFileUploads,
        initSearchableSelects: initSearchableSelects,
        initAll: init
    };
})();
