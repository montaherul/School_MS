window.MarksEntry = {
    examId: 0,
    subjectId: 0,
    components: [],
    saveTimers: {},
    baseUrl: '',
    _csrfToken: '',

    init: function(examId, subjectId, components, baseUrl) {
        this.examId = examId;
        this.subjectId = subjectId;
        this.components = components || [];
        this.baseUrl = baseUrl || '';
        this._csrfToken = document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';

        var self = this;
        document.querySelectorAll('.score-input').forEach(function(input) {
            input.addEventListener('input', function() {
                var row = input.closest('tr');
                if (!row) return;
                if (row.getAttribute('data-locked') === 'true') return;
                self.recalcRow(row);
                self.scheduleSave(row);
            });
        });

        document.querySelectorAll('.score-input').forEach(function(input) {
            input.dispatchEvent(new Event('input'));
        });

        document.querySelectorAll('[data-locked="true"] .score-input').forEach(function(input) {
            input.disabled = true;
        });
    },

    getComponentValue: function(row, field, code) {
        var input = row.querySelector('.cmp-' + field);
        return input ? (parseFloat(input.value) || 0) : 0;
    },

    recalcRow: function(row) {
        var total = 0;
        this.components.forEach(function(c) {
            total += this.getComponentValue(row, c.field, c.componentCode);
        }, this);

        var totalCell = row.querySelector('.c-total');
        if (totalCell) totalCell.textContent = total.toFixed(2);

        var grade = 'F', gp = 0.00;
        if (total >= 80) { grade = 'A+'; gp = 5.00; }
        else if (total >= 70) { grade = 'A'; gp = 4.00; }
        else if (total >= 60) { grade = 'A-'; gp = 3.50; }
        else if (total >= 50) { grade = 'B'; gp = 3.00; }
        else if (total >= 40) { grade = 'C'; gp = 2.00; }
        else if (total >= 33) { grade = 'D'; gp = 1.00; }

        var gradeCell = row.querySelector('.c-grade');
        if (gradeCell) {
            gradeCell.textContent = grade;
            gradeCell.className = 'er-badge c-grade ' + (grade === 'F' ? 'er-badge--rejected' : 'er-badge--converted');
        }

        var gpCell = row.querySelector('.c-gp');
        if (gpCell) gpCell.textContent = gp.toFixed(2);
    },

    scheduleSave: function(row) {
        var studentId = row.getAttribute('data-student-id');
        if (!studentId) return;
        if (row.getAttribute('data-locked') === 'true') return;

        if (this.saveTimers[studentId]) {
            clearTimeout(this.saveTimers[studentId]);
        }

        this.saveTimers[studentId] = setTimeout(this.saveRow.bind(this, row), 2000);
    },

    _getCsrfToken: function() {
        if (!this._csrfToken) {
            this._csrfToken = document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
        }
        return this._csrfToken;
    },

    saveRow: function(row) {
        var studentId = row.getAttribute('data-student-id');
        if (!studentId) return;
        if (row.getAttribute('data-locked') === 'true') return;

        var mark = { examId: this.examId, subjectId: this.subjectId, studentId: parseInt(studentId), marksObtained: 0, componentValues: {} };
        var total = 0;

        this.components.forEach(function(c) {
            var val = parseFloat(row.querySelector('.cmp-' + c.field).value) || null;
            if (val !== null) {
                if (c.field.startsWith('cmp_')) {
                    mark.componentValues[c.componentCode] = val;
                } else {
                    mark[c.field] = val;
                }
                total += val;
            }
        });

        mark.marksObtained = total;

        var indicator = this.getSaveIndicator(row);
        indicator.className = 'save-indicator saving';
        indicator.textContent = 'Saving...';

        var self = this;
        var xhr = new XMLHttpRequest();
        xhr.open('POST', this.baseUrl + '/Marks/SaveRow', true);
        xhr.setRequestHeader('Content-Type', 'application/json');
        xhr.setRequestHeader('RequestVerificationToken', this._getCsrfToken());
        xhr.onload = function() {
            if (xhr.status >= 200 && xhr.status < 300) {
                var resp = JSON.parse(xhr.responseText);
                if (resp.success) {
                    indicator.className = 'save-indicator saved';
                    indicator.textContent = 'Saved';
                    setTimeout(function() {
                        indicator.className = 'save-indicator';
                        indicator.textContent = '';
                    }, 3000);
                } else {
                    indicator.className = 'save-indicator error';
                    indicator.textContent = resp.message || 'Error';
                    setTimeout(function() { self.scheduleSave(row); }, 5000);
                }
            } else {
                indicator.className = 'save-indicator error';
                indicator.textContent = 'Save failed';
                if (xhr.status !== 403 && xhr.status !== 401) {
                    setTimeout(function() { self.scheduleSave(row); }, 5000);
                }
            }
        };
        xhr.onerror = function() {
            indicator.className = 'save-indicator error';
            indicator.textContent = 'Network error';
            setTimeout(function() { self.scheduleSave(row); }, 5000);
        };
        xhr.send(JSON.stringify(mark));
    },

    getSaveIndicator: function(row) {
        var el = row.querySelector('.save-indicator');
        if (!el) {
            el = document.createElement('span');
            el.className = 'save-indicator';
            var cell = row.querySelector('td:last-child');
            if (cell) {
                cell.appendChild(el);
            } else {
                row.appendChild(el);
            }
        }
        return el;
    },

    saveAll: function() {
        var rows = document.querySelectorAll('#marksheetTable tbody tr');
        var marks = [];
        var self = this;

        rows.forEach(function(row) {
            var studentId = row.getAttribute('data-student-id');
            if (!studentId) return;

            var mark = { examId: self.examId, subjectId: self.subjectId, studentId: parseInt(studentId), marksObtained: 0, componentValues: {} };
            var total = 0;

            self.components.forEach(function(c) {
                var val = parseFloat(row.querySelector('.cmp-' + c.field).value) || null;
                if (val !== null) {
                    if (c.field.startsWith('cmp_')) {
                        mark.componentValues[c.componentCode] = val;
                    } else {
                        mark[c.field] = val;
                    }
                    total += val;
                }
            });

            mark.marksObtained = total;
            marks.push(mark);
        });

        if (marks.length === 0) return;

        var btn = document.getElementById('btnSaveMarks');
        if (btn) { btn.disabled = true; btn.textContent = 'Saving...'; }

        var xhr = new XMLHttpRequest();
        xhr.open('POST', this.baseUrl + '/Marks/Save', true);
        xhr.setRequestHeader('Content-Type', 'application/json');
        xhr.setRequestHeader('RequestVerificationToken', document.querySelector('input[name="__RequestVerificationToken"]')?.value || '');
        xhr.onload = function() {
            if (btn) { btn.disabled = false; btn.innerHTML = '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" class="me-2"><path d="M19 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11l5 5v11a2 2 0 0 1-2 2z"/><polyline points="17 21 17 13 7 13 7 21"/><polyline points="7 3 7 8 15 8"/></svg> Save Marksheet'; }
            if (xhr.status >= 200 && xhr.status < 300) {
                var resp = JSON.parse(xhr.responseText);
                if (resp.success) {
                    var indicator = document.getElementById('autoSaveIndicator');
                    if (indicator) indicator.textContent = 'All saved at ' + new Date().toLocaleTimeString();
                } else {
                    alert(resp.message || 'Error saving marks.');
                }
            } else {
                alert('Error saving marks.');
            }
        };
        xhr.onerror = function() {
            if (btn) { btn.disabled = false; btn.innerHTML = '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" class="me-2"><path d="M19 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11l5 5v11a2 2 0 0 1-2 2z"/><polyline points="17 21 17 13 7 13 7 21"/><polyline points="7 3 7 8 15 8"/></svg> Save Marksheet'; }
            alert('Network error.');
        };
        xhr.send(JSON.stringify({ examId: this.examId, subjectId: this.subjectId, marks: marks }));
    }
};
