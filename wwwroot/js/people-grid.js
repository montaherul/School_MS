/* ═══════════════════════════════════════════════════════════════════
   People Module — Unified PersonCell / Avatar / Status / Actions
   Shared across Student, Admission, Teacher, Employee, User, Role

   Also includes entity mobile card rendering system.
   ═══════════════════════════════════════════════════════════════════ */

/* ─────────────────────────────────────────────────────────────────
   ENTITY MOBILE CARD SYSTEM
   Dual-render: Tabulator table (desktop) + Card view (mobile/tablet)
   ───────────────────────────────────────────────────────────────── */

var MobileCardManager = {
    isMobile: function () {
        return window.matchMedia('(max-width: 991.98px)').matches;
    },
    init: function (opts) {
        var self = this;
        self.table = opts.table;
        self.cardContainerId = opts.cardContainerId;
        self.renderFn = opts.renderFn;
        self.tableContainer = opts.tableContainer || null;

        var mq = window.matchMedia('(max-width: 991.98px)');
        mq.addListener(function () { self.toggle(); });

        if (self.table) {
            self.table.on('dataLoaded', function (data) {
                if (self.isMobile() && data && data.length) {
                    self.showCards(data);
                }
            });
            self.table.on('dataChanged', function (data) {
                if (self.isMobile() && data) {
                    self.renderCards(data);
                }
            });
            self.table.on('pageChanged', function () {
                setTimeout(function () {
                    if (self.isMobile()) {
                        var d = [];
                        try { d = self.table.getData(); } catch (e) {}
                        self.renderCards(d);
                    }
                }, 200);
            });
        }

        // Initial check after a short delay to let Tabulator AJAX load
        setTimeout(function () {
            if (self.isMobile()) {
                var d = [];
                try { d = self.table ? self.table.getData() : []; } catch (e) {}
                if (d && d.length) {
                    self.showCards(d);
                }
            }
        }, 500);
    },
    toggle: function () {
        if (this.isMobile()) {
            var d = [];
            try { d = this.table ? this.table.getData() : []; } catch (e) {}
            if (d && d.length) {
                this.showCards(d);
            } else {
                this.hideTable();
                this.showEmpty();
            }
        } else {
            this.showTable();
            this.hideCards();
        }
    },
    showCards: function (data) {
        this.hideTable();
        this.renderCards(data);
    },
    hideTable: function () {
        var tables = document.querySelectorAll('.entity-desktop-table');
        for (var i = 0; i < tables.length; i++) tables[i].classList.add('hidden');
    },
    showTable: function () {
        var tables = document.querySelectorAll('.entity-desktop-table');
        for (var i = 0; i < tables.length; i++) tables[i].classList.remove('hidden');
    },
    hideCards: function () {
        var container = document.getElementById(this.cardContainerId);
        if (container) container.classList.remove('active');
    },
    showEmpty: function () {
        var container = document.getElementById(this.cardContainerId);
        if (!container) return;
        container.classList.add('active');
        container.innerHTML = '<div class="dash-empty"><i class="bi bi-people fs-1"></i><div class="dash-empty__title">Loading...</div><div class="dash-empty__sub">Please wait while data loads.</div></div>';
    },
    renderCards: function (data) {
        var container = document.getElementById(this.cardContainerId);
        if (!container) return;
        if (!data || !data.length) {
            container.innerHTML = '<div class="dash-empty"><i class="bi bi-people fs-1"></i><div class="dash-empty__title">No Records Found</div><div class="dash-empty__sub">Try adjusting your filters.</div></div>';
            container.classList.add('active');
            return;
        }
        var html = '<div class="entity-mobile-cards">';
        for (var i = 0; i < data.length; i++) {
            html += this.renderFn(data[i]);
        }
        html += '</div>';
        container.innerHTML = html;
        container.classList.add('active');
    }
};

/* Entity avatar badge helper */
function getDesignationBadge(des) {
    if (!des) return '<span class="text-muted">&mdash;</span>';
    var colorMap = {
        'principal': '#DAA520', 'vice principal': '#C0C0C0', 'assistant head': '#C0C0C0',
        'senior': '#1E90FF', 'lecturer': '#1B4D8C', 'teacher': '#1B4D8C',
        'accountant': '#2E8B57', 'librarian': '#800080',
        'office staff': '#808080', 'lab assistant': '#FF8C00',
        'driver': '#8B4513', 'guard': '#282828', 'support': '#6B7280'
    };
    var key = des.toLowerCase();
    var color = '#1B4D8C';
    for (var k in colorMap) {
        if (key.indexOf(k) >= 0) { color = colorMap[k]; break; }
    }
    return '<span class="status-badge" style="background:' + color + ';color:#fff;padding:2px 10px;border-radius:12px;display:inline-block;font-size:11px;font-weight:600;white-space:nowrap;">' + des + '</span>';
}

function getStaffBadge(isTeaching) {
    return isTeaching
        ? '<span class="entity-cell__badge entity-cell__badge--teaching">Teaching</span>'
        : '<span class="entity-cell__badge entity-cell__badge--staff">Staff</span>';
}

/* ─────────────────────────────────────────────────────────────────
   STUDENT MOBILE CARD
   ───────────────────────────────────────────────────────────────── */
function renderStudentMobileCard(d) {
    var photo = d.profilePicturePath
        ? '<img src="' + d.profilePicturePath + '" alt="' + escapeHtml(d.fullName) + '" onerror="this.src=\'/images/default-user.png\'" />'
        : '<span class="entity-mobile-card__initials">' + (d.fullName || '?').substring(0, 2).toUpperCase() + '</span>';
    var statusCls = STATUS_MAP[d.status] || 'adm-status--neutral';
    var detailsHtml = '';
    if (d.className) detailsHtml += '<div class="entity-mobile-card__info-row"><span class="entity-mobile-card__label">Class</span><span class="entity-mobile-card__value">' + escapeHtml(d.className) + '</span></div>';
    if (d.sectionName) detailsHtml += '<div class="entity-mobile-card__info-row"><span class="entity-mobile-card__label">Section</span><span class="entity-mobile-card__value">' + escapeHtml(d.sectionName) + '</span></div>';
    if (d.rollNumber) detailsHtml += '<div class="entity-mobile-card__info-row"><span class="entity-mobile-card__label">Roll</span><span class="entity-mobile-card__value">' + escapeHtml(d.rollNumber.toString()) + '</span></div>';
    if (d.groupName) detailsHtml += '<div class="entity-mobile-card__info-row"><span class="entity-mobile-card__label">Group</span><span class="entity-mobile-card__value">' + escapeHtml(d.groupName) + '</span></div>';
    if (d.fatherName) detailsHtml += '<div class="entity-mobile-card__info-row"><span class="entity-mobile-card__label">Guardian</span><span class="entity-mobile-card__value">' + escapeHtml(d.fatherName) + '</span></div>';
    if (d.mobileNumber) detailsHtml += '<div class="entity-mobile-card__info-row"><span class="entity-mobile-card__label">Mobile</span><span class="entity-mobile-card__value">' + escapeHtml(d.mobileNumber) + '</span></div>';
    if (d.bloodGroup) detailsHtml += '<div class="entity-mobile-card__info-row"><span class="entity-mobile-card__label">Blood Group</span><span class="entity-mobile-card__value">' + escapeHtml(d.bloodGroup) + '</span></div>';
    if (d.admissionYear) detailsHtml += '<div class="entity-mobile-card__info-row"><span class="entity-mobile-card__label">Admission Year</span><span class="entity-mobile-card__value">' + escapeHtml(d.admissionYear.toString()) + '</span></div>';

    var cardId = 'student-card-' + (d.id || d.studentNo || Math.random());
    return '<div class="entity-mobile-card" id="' + cardId + '">'
        + '<div class="entity-mobile-card__top">'
        + '<div class="entity-mobile-card__photo">' + photo + '</div>'
        + '<div class="entity-mobile-card__identity">'
        + '<div class="entity-mobile-card__name">' + escapeHtml(d.fullName) + '</div>'
        + '<div class="entity-mobile-card__code">' + escapeHtml(d.studentNo || '') + '</div>'
        + '<div class="entity-mobile-card__status"><span class="adm-status ' + statusCls + '">' + escapeHtml(d.status) + '</span></div>'
        + '</div></div>'
        + '<button class="entity-mobile-card__expand-btn" onclick="toggleCardDetails(\'' + cardId + '\')"><span>Student Details</span><i class="bi bi-chevron-down"></i></button>'
        + '<div class="entity-mobile-card__body">' + detailsHtml + '</div>'
        + '<div class="entity-mobile-card__actions">'
        + '<a href="/Student/Details/' + (d.studentNo || d.id) + '" class="entity-mobile-card__action entity-mobile-card__action--view"><i class="bi bi-eye"></i> View</a>'
        + '<a href="/Student/CreateEdit/' + d.id + '" class="entity-mobile-card__action entity-mobile-card__action--edit"><i class="bi bi-pencil"></i> Edit</a>'
        + '<button type="button" class="entity-mobile-card__action entity-mobile-card__action--delete" onclick="openDeleteModal(\'' + escapeHtml(d.fullName) + '\',' + d.id + ')"><i class="bi bi-trash"></i> Delete</button>'
        + '</div></div>';
}

/* ─────────────────────────────────────────────────────────────────
   TEACHER MOBILE CARD
   ───────────────────────────────────────────────────────────────── */
function renderTeacherMobileCard(d) {
    var photo = d.profilePicturePath
        ? '<img src="' + d.profilePicturePath + '" alt="' + escapeHtml(d.fullName) + '" onerror="this.src=\'/images/default-user.png\'" />'
        : '<span class="entity-mobile-card__initials">' + (d.fullName || '?').substring(0, 2).toUpperCase() + '</span>';
    var statusCls = STATUS_MAP[d.status] || 'adm-status--neutral';
    var detailsHtml = '';
    detailsHtml += '<div class="entity-mobile-card__info-row"><span class="entity-mobile-card__label">Designation</span><span class="entity-mobile-card__value">' + getDesignationBadge(d.designation || d.designationName) + '</span></div>';
    if (d.department || d.departmentName) detailsHtml += '<div class="entity-mobile-card__info-row"><span class="entity-mobile-card__label">Department</span><span class="entity-mobile-card__value">' + escapeHtml(d.department || d.departmentName) + '</span></div>';
    if (d.mobileNumber) detailsHtml += '<div class="entity-mobile-card__info-row"><span class="entity-mobile-card__label">Mobile</span><span class="entity-mobile-card__value">' + escapeHtml(d.mobileNumber) + '</span></div>';
    if (d.email) detailsHtml += '<div class="entity-mobile-card__info-row"><span class="entity-mobile-card__label">Email</span><span class="entity-mobile-card__value">' + escapeHtml(d.email) + '</span></div>';

    var cardId = 'teacher-card-' + d.id;
    var actionsHtml = ''
        + '<a href="/Teacher/Details/' + d.id + '" class="entity-mobile-card__action entity-mobile-card__action--view"><i class="bi bi-eye"></i> View</a>'
        + '<a href="/Teacher/Edit/' + d.id + '" class="entity-mobile-card__action entity-mobile-card__action--edit"><i class="bi bi-pencil"></i> Edit</a>';
    if (d.status === 'Active') {
        actionsHtml += '<button type="button" class="entity-mobile-card__action entity-mobile-card__action--delete" onclick="if(confirm(\'Deactivate this teacher?\')){postAction(\'/Teacher/Deactivate\',' + d.id + ')}"><i class="bi bi-person-x"></i> Deactivate</button>';
    } else {
        actionsHtml += '<button type="button" class="entity-mobile-card__action entity-mobile-card__action--success" onclick="postAction(\'/Teacher/Activate\',' + d.id + ')"><i class="bi bi-person-check"></i> Activate</button>';
    }
    var photoHtml = '<div class="entity-mobile-card__photo">' + photo + '</div>';
    var identityHtml = '<div class="entity-mobile-card__identity">'
        + '<div class="entity-mobile-card__name">' + escapeHtml(d.fullName) + '</div>'
        + '<div class="entity-mobile-card__code">' + escapeHtml(d.teacherNo || '') + '</div>'
        + '<div class="entity-mobile-card__status"><span class="adm-status ' + statusCls + '">' + escapeHtml(d.status) + '</span></div>'
        + '</div>';
    var expandBtn = '<button class="entity-mobile-card__expand-btn" onclick="toggleCardDetails(\'' + cardId + '\')"><span>Details</span><i class="bi bi-chevron-down"></i></button>';
    return '<div class="entity-mobile-card" id="' + cardId + '">'
        + '<div class="entity-mobile-card__top">' + photoHtml + identityHtml + '</div>'
        + expandBtn
        + '<div class="entity-mobile-card__body">' + detailsHtml + '</div>'
        + '<div class="entity-mobile-card__actions">' + actionsHtml + '</div></div>';
}

/* ─────────────────────────────────────────────────────────────────
   EMPLOYEE MOBILE CARD
   ───────────────────────────────────────────────────────────────── */
function renderEmployeeMobileCard(d) {
    var photo = d.profilePicturePath
        ? '<img src="' + d.profilePicturePath + '" alt="' + escapeHtml(d.fullName) + '" onerror="this.src=\'/images/default-user.png\'" />'
        : '<span class="entity-mobile-card__initials">' + (d.fullName || '?').substring(0, 2).toUpperCase() + '</span>';
    var statusCls = STATUS_MAP[d.status] || 'adm-status--neutral';
    var staffBadge = d.isTeachingStaff !== undefined ? getStaffBadge(d.isTeachingStaff) : '';
    var detailsHtml = '';
    if (d.designation || d.designationName) detailsHtml += '<div class="entity-mobile-card__info-row"><span class="entity-mobile-card__label">Designation</span><span class="entity-mobile-card__value">' + getDesignationBadge(d.designation || d.designationName) + '</span></div>';
    if (d.department || d.departmentName) detailsHtml += '<div class="entity-mobile-card__info-row"><span class="entity-mobile-card__label">Department</span><span class="entity-mobile-card__value">' + escapeHtml(d.department || d.departmentName) + '</span></div>';
    if (d.phone || d.mobileNumber) detailsHtml += '<div class="entity-mobile-card__info-row"><span class="entity-mobile-card__label">Phone</span><span class="entity-mobile-card__value">' + escapeHtml(d.phone || d.mobileNumber) + '</span></div>';
    if (d.email) detailsHtml += '<div class="entity-mobile-card__info-row"><span class="entity-mobile-card__label">Email</span><span class="entity-mobile-card__value">' + escapeHtml(d.email) + '</span></div>';

    var cardId = 'emp-card-' + d.id;
    return '<div class="entity-mobile-card" id="' + cardId + '">'
        + '<div class="entity-mobile-card__top">'
        + '<div class="entity-mobile-card__photo">' + photo + '</div>'
        + '<div class="entity-mobile-card__identity">'
        + '<div class="entity-mobile-card__name">' + escapeHtml(d.fullName) + '</div>'
        + '<div class="entity-mobile-card__code">' + escapeHtml(d.employeeCode || '') + '</div>'
        + (staffBadge ? '<div style="margin-top:2px;">' + staffBadge + '</div>' : '')
        + '<div class="entity-mobile-card__status" style="margin-top:2px;"><span class="adm-status ' + statusCls + '">' + escapeHtml(d.status) + '</span></div>'
        + '</div></div>'
        + '<button class="entity-mobile-card__expand-btn" onclick="toggleCardDetails(\'' + cardId + '\')"><span>Details</span><i class="bi bi-chevron-down"></i></button>'
        + '<div class="entity-mobile-card__body">' + detailsHtml + '</div>'
        + '<div class="entity-mobile-card__actions">'
        + '<a href="/Employee/Details/' + d.id + '" class="entity-mobile-card__action entity-mobile-card__action--view"><i class="bi bi-eye"></i> View</a>'
        + '<a href="/Employee/Edit/' + d.id + '" class="entity-mobile-card__action entity-mobile-card__action--edit"><i class="bi bi-pencil"></i> Edit</a>'
        + '</div></div>';
}

/* ─────────────────────────────────────────────────────────────────
   ADMISSION MOBILE CARD
   ───────────────────────────────────────────────────────────────── */
function renderAdmissionMobileCard(d) {
    var photo = d.profilePicturePath
        ? '<img src="' + d.profilePicturePath + '" alt="' + escapeHtml(d.applicantName) + '" onerror="this.src=\'/images/default-user.png\'" />'
        : '<span class="entity-mobile-card__initials">' + (d.applicantName || '?').substring(0, 2).toUpperCase() + '</span>';
    var statusCls = STATUS_MAP[d.status] || 'adm-status--neutral';
    var detailsHtml = '';
    if (d.applicantNameBangla) detailsHtml += '<div class="entity-mobile-card__info-row"><span class="entity-mobile-card__label">নাম (বাংলা)</span><span class="entity-mobile-card__value adm-cell-bangla" lang="bn">' + escapeHtml(d.applicantNameBangla) + '</span></div>';
    if (d.className) detailsHtml += '<div class="entity-mobile-card__info-row"><span class="entity-mobile-card__label">Applied Class</span><span class="entity-mobile-card__value">' + escapeHtml(d.className) + '</span></div>';
    if (d.gender) detailsHtml += '<div class="entity-mobile-card__info-row"><span class="entity-mobile-card__label">Gender</span><span class="entity-mobile-card__value">' + escapeHtml(d.gender) + '</span></div>';
    if (d.applicantMobileNumber) detailsHtml += '<div class="entity-mobile-card__info-row"><span class="entity-mobile-card__label">Phone</span><span class="entity-mobile-card__value">' + escapeHtml(d.applicantMobileNumber) + '</span></div>';
    if (d.guardianName) detailsHtml += '<div class="entity-mobile-card__info-row"><span class="entity-mobile-card__label">Guardian</span><span class="entity-mobile-card__value">' + escapeHtml(d.guardianName) + '</span></div>';
    if (d.dateOfBirth) detailsHtml += '<div class="entity-mobile-card__info-row"><span class="entity-mobile-card__label">DOB / Age</span><span class="entity-mobile-card__value">' + new Date(d.dateOfBirth).toLocaleDateString('en-BD') + ' (' + (d.age || '') + ' yrs)</span></div>';

    var cardId = 'adm-card-' + d.id;
    var actionsHtml = '<a href="/Admission/Details/' + d.id + '" class="entity-mobile-card__action entity-mobile-card__action--view"><i class="bi bi-eye"></i> Details</a>';
    if (d.status === 'Pending') {
        actionsHtml += '<button type="button" class="entity-mobile-card__action entity-mobile-card__action--success" onclick="openApproveModal(' + d.id + ', ' + (d.appliedClassId || 0) + ')"><i class="bi bi-check-lg"></i> Convert</button>'
            + '<button type="button" class="entity-mobile-card__action entity-mobile-card__action--delete" onclick="if(confirm(\'Reject this application?\')){rejectItem(' + d.id + ')}"><i class="bi bi-x-lg"></i> Reject</button>';
    } else {
        actionsHtml += '<span class="entity-mobile-card__action" style="color:var(--adm-text-3);cursor:default;grid-column:span 2;"><i class="bi bi-check2-circle"></i> ' + escapeHtml(d.status) + '</span>';
    }
    return '<div class="entity-mobile-card" id="' + cardId + '">'
        + '<div class="entity-mobile-card__top">'
        + '<div class="entity-mobile-card__photo">' + photo + '</div>'
        + '<div class="entity-mobile-card__identity">'
        + '<div class="entity-mobile-card__name">' + escapeHtml(d.applicantName) + '</div>'
        + '<div class="entity-mobile-card__code">' + escapeHtml(d.applicationNo || '') + '</div>'
        + '<div class="entity-mobile-card__status"><span class="adm-status ' + statusCls + '">' + escapeHtml(d.status) + '</span></div>'
        + '</div></div>'
        + '<button class="entity-mobile-card__expand-btn" onclick="toggleCardDetails(\'' + cardId + '\')"><span>Application Details</span><i class="bi bi-chevron-down"></i></button>'
        + '<div class="entity-mobile-card__body">' + detailsHtml + '</div>'
        + '<div class="entity-mobile-card__actions">' + actionsHtml + '</div></div>';
}

/* ─────────────────────────────────────────────────────────────────
   UTILITY: toggle expandable card details
   ───────────────────────────────────────────────────────────────── */
function toggleCardDetails(cardId) {
    var card = document.getElementById(cardId);
    if (!card) return;
    var body = card.querySelector('.entity-mobile-card__body');
    var btn = card.querySelector('.entity-mobile-card__expand-btn');
    if (!body || !btn) return;
    body.classList.toggle('is-open');
    btn.classList.toggle('is-open');
}

/* ─────────────────────────────────────────────────────────────────
   UTILITY: HTML escaping
   ───────────────────────────────────────────────────────────────── */
function escapeHtml(str) {
    if (!str) return '';
    return String(str)
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#39;');
}

function renderAvatar(src, name) {
    if (src) {
        return '<div class="entity-avatar entity-avatar--primary"><img src="' + src + '" alt="" onerror="this.src=\'/images/default-user.png\'" /></div>';
    }
    var initials = (name || '?').substring(0, 2).toUpperCase();
    return '<div class="entity-avatar entity-avatar--primary"><span class="entity-avatar__initials">' + initials + '</span></div>';
}

function renderPersonCell(data, opts) {
    var name = data[opts.nameField] || '-';
    var code = data[opts.codeField] || '';
    var meta = opts.metaField ? (data[opts.metaField] || '') : '';
    var avatarField = opts.avatarField || 'profilePicturePath';
    var avatar = renderAvatar(data[avatarField], name);
    var nameHtml = '<span class="entity-cell__name">' + name + '</span>';
    var codeHtml = code ? '<span class="entity-cell__code">' + code + '</span>' : '';
    var metaHtml = meta ? '<span class="entity-cell__meta">' + meta + '</span>' : '';
    return '<div class="entity-cell">' + avatar + '<div class="entity-cell__identity">' + nameHtml + codeHtml + metaHtml + '</div></div>';
}

function renderActionBtn(url, className, icon, title) {
    return '<a href="' + url + '" class="action-btn ' + className + '" title="' + title + '"><i class="bi bi-' + icon + '"></i></a>';
}

function renderActionButton(className, icon, title, onClick) {
    return '<button type="button" class="action-btn ' + className + '" title="' + title + '" onclick="' + onClick + '"><i class="bi bi-' + icon + '"></i></button>';
}

function buildActionGroup(items) {
    var html = '<div class="action-group">';
    for (var i = 0; i < items.length; i++) {
        var item = items[i];
        if (item.url) {
            html += renderActionBtn(item.url, item.className, item.icon, item.title);
        } else if (item.onClick) {
            html += renderActionButton(item.className, item.icon, item.title, item.onClick);
        }
    }
    return html + '</div>';
}

var STATUS_MAP = {
    'Active': 'adm-status--approved',
    'active': 'adm-status--approved',
    'Inactive': 'adm-status--rejected',
    'inactive': 'adm-status--rejected',
    'Pending': 'adm-status--pending',
    'pending': 'adm-status--pending',
    'Approved': 'adm-status--approved',
    'approved': 'adm-status--approved',
    'Converted': 'adm-status--converted',
    'converted': 'adm-status--converted',
    'Rejected': 'adm-status--rejected',
    'rejected': 'adm-status--rejected',
    'Resigned': 'adm-status--rejected',
    'Retired': 'adm-status--rejected',
    'On Leave': 'adm-status--pending',
    'Locked': 'adm-status--neutral',
    'locked': 'adm-status--neutral'
};

function renderStatusBadge(value) {
    if (value == null || value === '') return '<span class="adm-status adm-status--neutral">Unknown</span>';
    var cls = STATUS_MAP[value] || 'adm-status--neutral';
    return '<span class="adm-status ' + cls + '">' + value + '</span>';
}

var USER_STATUS_MAP = {
    1: { text: 'Active', cls: 'adm-status--approved' },
    2: { text: 'Inactive', cls: 'adm-status--rejected' },
    3: { text: 'Locked', cls: 'adm-status--neutral' },
    4: { text: 'Pending', cls: 'adm-status--pending' }
};

function renderUserStatusBadge(value) {
    var entry = USER_STATUS_MAP[value] || { text: 'Unknown', cls: 'adm-status--neutral' };
    return '<span class="adm-status ' + entry.cls + '">' + entry.text + '</span>';
}
