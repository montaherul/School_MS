/* ═══════════════════════════════════════════════════════════════════
   People Module — Unified PersonCell / Avatar / Status / Actions
   Shared across Student, Admission, Teacher, Employee, User, Role
   ═══════════════════════════════════════════════════════════════════ */

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
