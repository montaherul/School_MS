window.RoutineGrid = (function () {
    'use strict';

    var table = null;

    function initialize(tableId, options) {
        if (!options) options = {};
        table = new Tabulator(tableId, {
            height: false,
            layout: 'fitColumns',
            movableColumns: false,
            movableRows: true,
            movableRowsConnectedTables: '.routine-drop-target',
            movableRowsSender: function (fromRow, toRow, fromTable, toTable) {
                return true;
            },
            movableRowsReceiver: function (toRow, fromRow, toTable, fromTable) {
                return true;
            },
            movableRowsEnd: function () {
            },
            pagination: true,
            paginationMode: 'remote',
            paginationSize: options.pageSize || 25,
            paginationSizeSelector: [10, 25, 50, 100],
            ajaxURL: options.ajaxURL || '/Routine/GetTimetableEntries',
            ajaxParams: options.ajaxParams || {},
            paginationDataSent: { page: 'page', size: 'pageSize' },
            ajaxResponse: function (url, params, response) {
                return { data: response.data || [], last_page: response.last_page || 1 };
            },
            columns: options.columns || [],
            placeholder: options.placeholder || '<div class="dash-empty">No entries found</div>',
            rowDblClick: function (e, row) {
                var data = row.getData();
                editEntry(data.id);
            },
            cellDblClick: function (e, cell) {
            }
        });
        return table;
    }

    function swapEntries(entryId1, entryId2, callback) {
        $.ajax({
            url: '/Routine/SwapEntries',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ entryId1: entryId1, entryId2: entryId2 }),
            headers: { RequestVerificationToken: getAntiForgeryToken() },
            success: function (response) {
                if (response.success) {
                    if (table) table.replaceData();
                    showToast('success', response.message || 'Entries swapped');
                    if (callback) callback(null, response);
                } else {
                    showToast('error', response.message || 'Swap failed');
                    if (callback) callback(response.message);
                }
            },
            error: function (xhr) {
                showToast('error', 'Server error: ' + xhr.statusText);
                if (callback) callback('Server error: ' + xhr.statusText);
            }
        });
    }

    function moveEntry(entryId, targetPeriodId, targetDayNumber, callback) {
        $.ajax({
            url: '/Routine/MoveEntry',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ entryId: entryId, targetPeriodId: targetPeriodId, targetDayNumber: targetDayNumber }),
            headers: { RequestVerificationToken: getAntiForgeryToken() },
            success: function (response) {
                if (response.success) {
                    if (table) table.replaceData();
                    showToast('success', response.message || 'Entry moved');
                    if (callback) callback(null, response);
                } else {
                    showToast('error', response.message || 'Failed to move entry');
                    if (callback) callback(response.message);
                }
            }
        });
    }

    function createEntry(data, callback) {
        $.ajax({
            url: '/Routine/CreateEntry',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            headers: { RequestVerificationToken: getAntiForgeryToken() },
            success: function (response) {
                if (response.success) {
                    if (table) table.replaceData();
                    showToast('success', response.message || 'Entry created');
                    if (callback) callback(null, response);
                } else {
                    showToast('error', response.message || 'Failed to create entry');
                    if (callback) callback(response.message);
                }
            }
        });
    }

    function updateEntry(id, roomId, routinePeriodId, dayNumber, callback) {
        $.ajax({
            url: '/Routine/UpdateEntry',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ id: id, roomId: roomId, routinePeriodId: routinePeriodId, dayNumber: dayNumber }),
            headers: { RequestVerificationToken: getAntiForgeryToken() },
            success: function (response) {
                if (response.success) {
                    if (table) table.replaceData();
                    if (callback) callback(null, response);
                } else {
                    showToast('error', response.message || 'Update failed');
                    if (callback) callback(response.message);
                }
            }
        });
    }

    function fullUpdateEntry(data, callback) {
        $.ajax({
            url: '/Routine/FullUpdateEntry',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            headers: { RequestVerificationToken: getAntiForgeryToken() },
            success: function (response) {
                if (response.success) {
                    if (table) table.replaceData();
                    if (callback) callback(null, response);
                } else {
                    showToast('error', response.message || 'Full update failed');
                    if (callback) callback(response.message);
                }
            }
        });
    }

    function saveEntry() {
        var id = $('#entryId').val();
        var data = {
            id: id || 0,
            academicYearId: parseInt($('#entryAcademicYearId').val()) || 0,
            classId: parseInt($('#entryClassId').val()) || 0,
            sectionId: parseInt($('#entrySectionId').val()) || 0,
            groupId: parseInt($('#entryGroupId').val()) || null,
            subjectId: parseInt($('#entrySubjectId').val()) || 0,
            teacherId: parseInt($('#entryTeacherId').val()) || 0,
            roomId: parseInt($('#entryRoomId').val()) || 0,
            routinePeriodId: parseInt($('#entryPeriodId').val()) || 0,
            dayNumber: parseInt($('#entryDayNumber').val()) || 1,
            isLab: $('#entryIsLab').is(':checked'),
            note: $('#entryNote').val() || ''
        };

        if (!data.classId || !data.subjectId || !data.teacherId || !data.routinePeriodId) {
            showToast('error', 'Please fill all required fields.');
            return;
        }

        var success = function(err, resp) {
            if (err) {
                showToast('error', err);
                return;
            }
            closeModal('entryModal');
            showToast('success', resp.message || (id ? 'Entry updated' : 'Entry created'));
            if (!id && table) table.replaceData();
        };

        if (id) {
            fullUpdateEntry(data, success);
        } else {
            createEntry(data, success);
        }
    }

    function deleteEntry(id, callback) {
        $.ajax({
            url: '/Routine/DeleteEntry/' + id,
            type: 'POST',
            headers: { RequestVerificationToken: getAntiForgeryToken() },
            success: function (response) {
                if (response.success) {
                    if (table) table.deleteRow(id);
                    showToast('success', response.message || 'Entry deleted');
                    if (callback) callback(null, response);
                } else {
                    showToast('error', response.message || 'Failed to delete entry');
                    if (callback) callback(response.message);
                }
            }
        });
    }

    function validateEntry(data, callback) {
        $.ajax({
            url: '/Routine/ValidateEntry',
            type: 'GET',
            data: data,
            success: function (response) {
                if (callback) callback(response);
            }
        });
    }

    function applyFilters(filters) {
        if (table) {
            table.setData();
        }
    }

    function reloadTable() {
        if (table) {
            table.replaceData();
        }
    }

    function getAntiForgeryToken() {
        var el = document.querySelector('input[name="__RequestVerificationToken"]');
        return el ? el.value : '';
    }

    function showToast(type, message) {
        if (typeof toastr !== 'undefined') {
            var fn = toastr[type] || toastr.info;
            fn(message, '', { timeOut: 3000, closeButton: true, progressBar: true });
            return;
        }
        var toast = document.createElement('div');
        toast.className = 'adm-toast adm-toast--' + type;
        toast.textContent = message;
        document.body.appendChild(toast);
        setTimeout(function () { toast.remove(); }, 3000);
    }

    function editEntry(id) {
        $.get('/Routine/GetEntry/' + id, function (resp) {
            var data = resp.data || resp;
            $('#entryId').val(data.id);
            $('#entryAcademicYearId').val(data.academicYearId);
            $('#entryClassId').val(data.classId);
            $('#entrySectionId').val(data.sectionId);
            $('#entryGroupId').val(data.groupId);
            $('#entrySubjectId').val(data.subjectId);
            $('#entryTeacherId').val(data.teacherId);
            $('#entryRoomId').val(data.roomId);
            $('#entryPeriodId').val(data.routinePeriodId);
            $('#entryDayNumber').val(data.dayNumber);
            $('#entryIsLab').prop('checked', data.isLab);
            $('#entryNote').val(data.note);
            $('#entryModal').addClass('adm-modal--open');
        });
    }

    function exportToExcel() {
        var params = table ? table.getAjaxParams() : {};
        var qs = Object.keys(params)
            .map(function (k) { return k + '=' + encodeURIComponent(params[k]); })
            .join('&');
        window.location.href = '/Routine/ExportExcel?' + qs;
    }

    function exportToPdf() {
        var params = table ? table.getAjaxParams() : {};
        var qs = Object.keys(params)
            .map(function (k) { return k + '=' + encodeURIComponent(params[k]); })
            .join('&');
        window.location.href = '/Routine/ExportPdf?' + qs;
    }

    function initDaySelector(containerId, onChange) {
        var days = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
        var html = '<div class="adm-btn-group" role="group">';
        html += '<button type="button" class="adm-btn adm-btn--ghost day-filter active" data-value="">All</button>';
        days.forEach(function (d, i) {
            html += '<button type="button" class="adm-btn adm-btn--ghost day-filter" data-value="' + i + '">' + d + '</button>';
        });
        html += '</div>';
        document.getElementById(containerId).innerHTML = html;

        document.querySelectorAll('.day-filter').forEach(function (btn) {
            btn.addEventListener('click', function () {
                document.querySelectorAll('.day-filter').forEach(function (b) { b.classList.remove('active'); });
                this.classList.add('active');
                if (onChange) onChange(this.getAttribute('data-value'));
            });
        });
    }

    function createVersion(data, callback) {
        $.ajax({
            url: '/Routine/CreateVersion',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            headers: { RequestVerificationToken: getAntiForgeryToken() },
            success: function (response) {
                if (response.success) {
                    showToast('success', response.message || 'Version created');
                    if (callback) callback(null, response);
                } else {
                    showToast('error', response.message || 'Failed to create version');
                    if (callback) callback(response.message);
                }
            }
        });
    }

    function publishVersion(versionId, callback) {
        $.ajax({
            url: '/Routine/PublishVersion/' + versionId,
            type: 'POST',
            headers: { RequestVerificationToken: getAntiForgeryToken() },
            success: function (response) {
                if (response.success) {
                    showToast('success', response.message || 'Version published');
                    if (callback) callback(null, response);
                } else {
                    showToast('error', response.message || 'Failed to publish version');
                    if (callback) callback(response.message);
                }
            }
        });
    }

    function approveVersion(versionId, callback) {
        $.ajax({
            url: '/Routine/ApproveVersion/' + versionId,
            type: 'POST',
            headers: { RequestVerificationToken: getAntiForgeryToken() },
            success: function (response) {
                if (response.success) {
                    showToast('success', response.message || 'Version approved');
                    if (callback) callback(null, response);
                } else {
                    showToast('error', response.message || 'Failed to approve version');
                    if (callback) callback(response.message);
                }
            }
        });
    }

    function generateRoutine(academicYearId, callback) {
        $.ajax({
            url: '/Routine/Generate',
            type: 'POST',
            data: { academicYearId: academicYearId },
            headers: { RequestVerificationToken: getAntiForgeryToken() },
            success: function (response) {
                if (response.success) {
                    showToast('success', response.message || 'Routine generated');
                    if (callback) callback(null, response);
                } else {
                    showToast('error', response.message || 'Generation failed');
                    if (callback) callback(response.message);
                }
            }
        });
    }

    function getGenerationStatus(generationId, callback) {
        $.ajax({
            url: '/Routine/GetGenerations?page=1&size=1',
            type: 'GET',
            success: function (response) {
                var list = response.data || [];
                var gen = list.length > 0 ? list[0] : null;
                if (callback) callback(gen);
            }
        });
    }

    function loadConflicts(containerId, generationId) {
        var url = generationId
            ? '/Routine/GetGenerationConflicts/' + generationId
            : '/Routine/GetConflicts';
        $.ajax({
            url: url,
            type: 'GET',
            success: function (response) {
                var container = document.getElementById(containerId);
                if (!container) return;
                var conflicts = response.data || response;
                if (!conflicts || (Array.isArray(conflicts) && conflicts.length === 0) || (conflicts.total === 0)) {
                    container.innerHTML = '<div class="dash-empty"><i class="bi bi-check-circle"></i><div class="dash-empty__title">No Conflicts</div><div class="dash-empty__sub">All entries are conflict-free.</div></div>';
                    return;
                }
                if (!Array.isArray(conflicts)) conflicts = [];
                var html = '';
                conflicts.forEach(function (c) {
                    var cls = c.isResolved ? 'conflict-card conflict-card--resolved' : 'conflict-card';
                    var badgeCls = c.isResolved ? 'adm-status adm-status--approved' : 'adm-status adm-status--rejected';
                    html += '<div class="' + cls + '">'
                        + '<div class="conflict-card__header">'
                        + '<span class="' + badgeCls + '">' + (c.isResolved ? 'Resolved' : 'Unresolved') + '</span>'
                        + '<span class="conflict-card__type">' + escapeHtml(c.conflictType) + '</span>'
                        + '</div>'
                        + '<div class="conflict-card__desc">' + escapeHtml(c.description) + '</div>'
                        + '<div class="conflict-card__meta">'
                        + (c.teacherName ? '<span><i class="bi bi-person"></i> ' + escapeHtml(c.teacherName) + '</span>' : '')
                        + (c.roomNo ? '<span><i class="bi bi-door-open"></i> ' + escapeHtml(c.roomNo) + '</span>' : '')
                        + (c.periodName ? '<span><i class="bi bi-clock"></i> ' + escapeHtml(c.periodName) + '</span>' : '')
                        + '</div>'
                        + '</div>';
                });
                container.innerHTML = html;
            }
        });
    }

    function bulkDelete(ids, callback) {
        $.ajax({
            url: '/Routine/BulkDeleteEntries',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ ids: ids }),
            headers: { RequestVerificationToken: getAntiForgeryToken() },
            success: function (response) {
                if (response.success) {
                    if (table) table.replaceData();
                    showToast('success', response.message || ids.length + ' entries deleted');
                    if (callback) callback(null, response);
                } else {
                    showToast('error', response.message || 'Bulk delete failed');
                    if (callback) callback(response.message);
                }
            }
        });
    }

    function bulkUpdate(ids, roomId, routinePeriodId, dayNumber, callback) {
        $.ajax({
            url: '/Routine/BulkUpdateEntries',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ ids: ids, roomId: roomId, routinePeriodId: routinePeriodId, dayNumber: dayNumber }),
            headers: { RequestVerificationToken: getAntiForgeryToken() },
            success: function (response) {
                if (response.success) {
                    if (table) table.replaceData();
                    showToast('success', response.message || ids.length + ' entries updated');
                    if (callback) callback(null, response);
                } else {
                    showToast('error', response.message || 'Bulk update failed');
                    if (callback) callback(response.message);
                }
            }
        });
    }

    function createPeriod(data, callback) {
        if (callback) callback('Endpoint not available via AJAX. Use form POST.');
    }

    function updatePeriod(id, data, callback) {
        if (callback) callback('Endpoint not available via AJAX. Use form POST.');
    }

    function deletePeriod(id, callback) {
        $.ajax({
            url: '/Routine/DeletePeriod/' + id,
            type: 'POST',
            headers: { RequestVerificationToken: getAntiForgeryToken() },
            success: function (response) {
                if (response.success) {
                    showToast('success', 'Period deleted');
                    if (callback) callback(null, response);
                } else {
                    showToast('error', response.message || 'Failed to delete period');
                    if (callback) callback(response.message);
                }
            }
        });
    }

    function createRoom(data, callback) {
        if (callback) callback('Endpoint not available via AJAX. Use form POST.');
    }

    function updateRoom(id, data, callback) {
        if (callback) callback('Endpoint not available via AJAX. Use form POST.');
    }

    function deleteRoom(id, callback) {
        $.ajax({
            url: '/Routine/DeleteRoom/' + id,
            type: 'POST',
            headers: { RequestVerificationToken: getAntiForgeryToken() },
            success: function (response) {
                if (response.success) {
                    showToast('success', 'Room deleted');
                    if (callback) callback(null, response);
                } else {
                    showToast('error', response.message || 'Failed to delete room');
                    if (callback) callback(response.message);
                }
            }
        });
    }

    function createSubjectRequirement(data, callback) {
        if (callback) callback('Endpoint not available via AJAX. Use form POST.');
    }

    function updateSubjectRequirement(id, data, callback) {
        if (callback) callback('Endpoint not available via AJAX. Use form POST.');
    }

    function deleteSubjectRequirement(id, callback) {
        $.ajax({
            url: '/Routine/DeleteSubjectRequirement/' + id,
            type: 'POST',
            headers: { RequestVerificationToken: getAntiForgeryToken() },
            success: function (response) {
                if (response.success) {
                    showToast('success', 'Subject requirement deleted');
                    if (callback) callback(null, response);
                } else {
                    showToast('error', response.message || 'Failed to delete subject requirement');
                    if (callback) callback(response.message);
                }
            }
        });
    }

    function saveTeacherAvailability(data, callback) {
        if (callback) callback('Endpoint not available via AJAX.');
    }

    function saveWorkingDays(data, callback) {
        if (callback) callback('Endpoint not available via AJAX.');
    }

    function loadDashboardStats(containerId) {
        $.ajax({
            url: '/Routine/GetDashboardData',
            type: 'GET',
            success: function (response) {
                var container = document.getElementById(containerId);
                if (!container) return;
                var data = response.data || response;
                if (!data) {
                    container.innerHTML = '';
                    return;
                }
                var cards = [
                    { icon: 'bi-calendar-check', label: 'Total Entries', value: data.totalEntries || 0 },
                    { icon: 'bi-people', label: 'Teachers', value: data.totalTeachers || 0 },
                    { icon: 'bi-door-open', label: 'Rooms', value: data.totalRooms || 0 },
                    { icon: 'bi-book', label: 'Subjects', value: data.totalSubjects || 0 },
                    { icon: 'bi-exclamation-triangle', label: 'Conflicts', value: data.totalConflicts || 0 }
                ];
                var html = '';
                cards.forEach(function (c) {
                    html += '<div class="adm-stat-card">'
                        + '<div class="adm-stat-card__icon"><i class="bi ' + c.icon + '"></i></div>'
                        + '<div class="adm-stat-card__body">'
                        + '<div class="adm-stat-card__value">' + c.value + '</div>'
                        + '<div class="adm-stat-card__label">' + c.label + '</div>'
                        + '</div></div>';
                });
                container.innerHTML = html;
            }
        });
    }

    function escapeHtml(str) {
        if (!str) return '';
        return String(str)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;');
    }

    function populateSelect(selectId, items, valueField, textField, placeholder) {
        var sel = $('#' + selectId);
        sel.empty();
        sel.append('<option value="">' + (placeholder || '-- Select --') + '</option>');
        if (items && items.length) {
            items.forEach(function (item) {
                sel.append('<option value="' + item[valueField] + '">' + escapeHtml(item[textField]) + '</option>');
            });
        }
    }

    function cascadeSelects(options) {
        var baseUrl = options.baseUrl || '/Routine';

        function resetSelect(id) { $('#' + id).empty().append('<option value="">-- Select --</option>').prop('disabled', true); }

        $(document).on('change', '#' + options.classSelect, function () {
            var classId = $(this).val();
            if (options.groupSelect) resetSelect(options.groupSelect);
            if (options.sectionSelect) resetSelect(options.sectionSelect);
            if (options.subjectSelect) resetSelect(options.subjectSelect);
            if (options.teacherSelect) resetSelect(options.teacherSelect);
            if (!classId) return;

            var isGroupBased = $(this).find('option:selected').attr('data-is-group-based') === 'true';

            if (isGroupBased && options.groupSelect) {
                $.get(baseUrl + '/GetGroupsByClass?classId=' + classId, function (resp) {
                    var groups = resp.data || resp;
                    if (groups && groups.length) {
                        populateSelect(options.groupSelect, groups, 'id', 'name', '-- Choose Group --');
                        $('#' + options.groupSelect).prop('disabled', false);
                    } else {
                        loadSections(classId, null);
                    }
                });
            } else {
                loadSections(classId, null);
            }
        });

        if (options.groupSelect) {
            $(document).on('change', '#' + options.groupSelect, function () {
                var classId = $('#' + options.classSelect).val();
                var groupId = $(this).val();
                if (options.sectionSelect) resetSelect(options.sectionSelect);
                if (options.subjectSelect) resetSelect(options.subjectSelect);
                if (options.teacherSelect) resetSelect(options.teacherSelect);
                if (classId && groupId) loadSections(classId, groupId);
            });
        }

        if (options.sectionSelect) {
            $(document).on('change', '#' + options.sectionSelect, function () {
                var classId = $('#' + options.classSelect).val();
                var groupId = $('#' + options.groupSelect).val();
                var sectionId = $(this).val();
                if (options.subjectSelect) resetSelect(options.subjectSelect);
                if (options.teacherSelect) resetSelect(options.teacherSelect);
                if (classId && sectionId) loadSubjects(classId, groupId, sectionId);
            });
        }

        function loadSections(classId, groupId) {
            var url = baseUrl + '/GetSectionsByClass?classId=' + classId;
            if (groupId) url += '&groupId=' + groupId;
            $.get(url, function (resp) {
                var sections = resp.data || resp;
                if (sections && sections.length) {
                    populateSelect(options.sectionSelect, sections, 'id', 'name', '-- Choose Section --');
                    $('#' + options.sectionSelect).prop('disabled', false);
                }
            });
        }

        function loadSubjects(classId, groupId, sectionId) {
            var url = baseUrl + '/GetRequirementsForClass?classId=' + classId + '&sectionId=' + sectionId;
            if (groupId) url += '&groupId=' + groupId;
            $.get(url, function (resp) {
                var subjects = resp.data || resp;
                if (subjects && subjects.length) {
                    populateSelect(options.subjectSelect, subjects, 'id', 'name', '-- Choose Subject --');
                    $('#' + options.subjectSelect).prop('disabled', false);
                }
            });
        }
    }

    function openModal(modalId) {
        $('#' + modalId).addClass('adm-modal--open');
    }

    function closeModal(modalId) {
        $('#' + modalId).removeClass('adm-modal--open');
    }

    function resetModalForm(formId) {
        $('#' + formId)[0]?.reset();
        $('#' + formId + ' select').prop('disabled', true);
    }

    function checkConflicts(entryData) {
        $.ajax({
            url: '/Routine/ValidateEntry',
            type: 'GET',
            data: entryData,
            success: function (response) {
                if (response && response.isValid === false) {
                    showToast('warning', 'Conflict(s) detected');
                } else {
                    showToast('success', 'No conflicts detected');
                }
            }
        });
    }

    function renderTimetableView(containerId, data) {
        var container = document.getElementById(containerId);
        if (!container) return;
        if (!data || !data.periods || !data.entries) {
            container.innerHTML = '<div class="dash-empty">No timetable data available</div>';
            return;
        }

        var days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
        var dayMap = {};
        (data.workingDays || days).forEach(function (d, i) {
            if (d.isWorkingDay !== false) dayMap[i] = d.dayName || days[i];
        });

        var html = '<div class="routine-table-wrapper"><table class="routine-timetable"><thead><tr><th class="routine-timetable__period">Period</th>';
        Object.keys(dayMap).forEach(function (dayNum) {
            html += '<th class="routine-timetable__day">' + dayMap[dayNum] + '</th>';
        });
        html += '</tr></thead><tbody>';

        data.periods.forEach(function (p) {
            html += '<tr><td class="routine-timetable__period">'
                + '<div class="routine-timetable__period-name">' + escapeHtml(p.name) + '</div>'
                + '<div class="routine-timetable__period-time">' + p.startTime + ' - ' + p.endTime + '</div>'
                + '</td>';
            Object.keys(dayMap).forEach(function (dayNum) {
                var entry = null;
                if (data.entries[dayNum]) {
                    entry = data.entries[dayNum].find(function (e) { return e.routinePeriodId === p.id; });
                }
                if (entry) {
                    html += '<td class="routine-timetable__cell">'
                        + '<div class="routine-timetable__subject">' + escapeHtml(entry.subjectName) + '</div>'
                        + '<div class="routine-timetable__teacher">' + escapeHtml(entry.teacherName) + '</div>'
                        + '<div class="routine-timetable__room">' + escapeHtml(entry.roomNo) + '</div>'
                        + (entry.isLab ? '<span class="routine-timetable__badge">Lab</span>' : '')
                        + '</td>';
                } else {
                    html += '<td class="routine-timetable__cell routine-timetable__cell--empty">--</td>';
                }
            });
            html += '</tr>';
        });

        html += '</tbody></table></div>';
        container.innerHTML = html;
    }

    function getSelectedIds() {
        if (!table) return [];
        var rows = table.getSelectedRows();
        return rows.map(function (r) { return r.getData().id; });
    }

    return {
        initialize: initialize,
        reloadTable: reloadTable,
        applyFilters: applyFilters,

        swapEntries: swapEntries,
        moveEntry: moveEntry,
        createEntry: createEntry,
        updateEntry: updateEntry,
        deleteEntry: deleteEntry,
        validateEntry: validateEntry,
        editEntry: editEntry,
        saveEntry: saveEntry,
        fullUpdateEntry: fullUpdateEntry,
        bulkDelete: bulkDelete,
        bulkUpdate: bulkUpdate,

        createVersion: createVersion,
        publishVersion: publishVersion,
        approveVersion: approveVersion,

        generateRoutine: generateRoutine,
        getGenerationStatus: getGenerationStatus,

        loadConflicts: loadConflicts,
        checkConflicts: checkConflicts,

        createPeriod: createPeriod,
        updatePeriod: updatePeriod,
        deletePeriod: deletePeriod,

        createRoom: createRoom,
        updateRoom: updateRoom,
        deleteRoom: deleteRoom,

        createSubjectRequirement: createSubjectRequirement,
        updateSubjectRequirement: updateSubjectRequirement,
        deleteSubjectRequirement: deleteSubjectRequirement,

        saveTeacherAvailability: saveTeacherAvailability,
        saveWorkingDays: saveWorkingDays,

        loadDashboardStats: loadDashboardStats,

        renderTimetableView: renderTimetableView,

        initDaySelector: initDaySelector,
        showToast: showToast,
        openModal: openModal,
        closeModal: closeModal,
        resetModalForm: resetModalForm,
        populateSelect: populateSelect,
        cascadeSelects: cascadeSelects,
        escapeHtml: escapeHtml,

        exportToExcel: exportToExcel,
        exportToPdf: exportToPdf,

        getSelectedIds: getSelectedIds
    };
})();
