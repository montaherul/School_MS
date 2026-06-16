/* ═══════════════════════════════════════════════════════════════════
   SchoolMS — Academic Calendar Module (shared helpers)
   ═══════════════════════════════════════════════════════════════════ */

(function () {
  'use strict';

  window.AcademicCalendar = window.AcademicCalendar || {};

  /* ── Format helpers ─────────────────────────────────────── */
  AcademicCalendar.formatDate = function (d) {
    return d.getFullYear() + '-' +
      String(d.getMonth() + 1).padStart(2, '0') + '-' +
      String(d.getDate()).padStart(2, '0');
  };

  AcademicCalendar.formatDisplay = function (d) {
    return d.toLocaleDateString(undefined, {
      day: 'numeric', month: 'short', year: 'numeric'
    });
  };

  AcademicCalendar.formatDisplayShort = function (d) {
    return d.toLocaleDateString(undefined, {
      weekday: 'short', day: 'numeric', month: 'short'
    });
  };

  AcademicCalendar.dayNames = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
  AcademicCalendar.dayNamesShort = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
  AcademicCalendar.monthNames = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];

  /* ═══════════════════════════════════════════════════════════════
     FIX 3+4 — SHARED FILTER + MUTUALLY EXCLUSIVE CATEGORIES
     Priority: Holiday > Exam > Website Event > Academic Event > Working
     ═══════════════════════════════════════════════════════════════ */
  AcademicCalendar.activeFilter = 'all';

  AcademicCalendar.getClassify = function (item) {
    if (item.isHoliday) return 'holiday';
    if (item.source === 'exam_schedule' || item.source === 'exam' || item.isExamDay) return 'exam';
    if (item.isWebsiteEvent || item.source === 'website_event') return 'website_event';
    if (item.isEventDay) return 'academic';
    if (item.isWorkingDay) return 'working';
    if (item.type === 'holiday') return 'holiday';
    if (item.type === 'exam') return 'exam';
    if (item.type === 'event') return 'academic';
    if (item.type === 'working') return 'working';
    return 'working';
  };

  AcademicCalendar.matchesFilter = function (item) {
    var cat = AcademicCalendar.getClassify(item);
    var f = AcademicCalendar.activeFilter;
    if (f === 'all') return cat !== 'working';
    return cat === f;
  };

  AcademicCalendar.isWorkingDay = function (item) {
    return AcademicCalendar.getClassify(item) === 'working';
  };

  /* ── Badge / color helpers ─────────────────────────────── */
  AcademicCalendar.getTypeInfo = function (item) {
    var cat = AcademicCalendar.getClassify(item);
    switch (cat) {
      case 'holiday':       return { type: 'holiday',       label: 'Holiday',      css: 'holiday',       color: '#dc3545' };
      case 'exam':          return { type: 'exam',          label: 'Exam',         css: 'exam',          color: '#0d6efd' };
      case 'website_event': return { type: 'website_event', label: 'Website Event',css: 'webevent',      color: '#6f42c1', shortLabel: 'EVENT' };
      case 'academic':      return { type: 'event',         label: 'Academic Event',css: 'event',         color: '#fd7e14', shortLabel: 'EVENT' };
      default:              return { type: 'working',       label: 'Working Day',   css: 'working',       color: '#198754' };
    }
  };

  AcademicCalendar.getTypeInfoFromAgenda = function (item) {
    var cat = item.source === 'website_event' ? 'website_event'
            : item.source === 'exam_schedule' || item.source === 'exam' ? 'exam'
            : item.type || AcademicCalendar.getClassify(item);
    switch (cat) {
      case 'holiday':       return { type: 'holiday',       label: 'Holiday',      css: 'holiday',       color: '#dc3545' };
      case 'exam':          return { type: 'exam',          label: 'Exam',         css: 'exam',          color: '#0d6efd', shortLabel: 'EXAM' };
      case 'website_event': return { type: 'website_event', label: 'Website Event',css: 'webevent',      color: '#6f42c1', shortLabel: 'EVENT' };
      case 'event':         return { type: 'event',         label: 'Academic Event',css: 'event',         color: '#fd7e14', shortLabel: 'EVENT' };
      default:              return { type: 'working',       label: 'Working Day',   css: 'working',       color: '#198754' };
    }
  };

  /* ── Day type dot generator ────────────────────────────── */
  AcademicCalendar.getDots = function (item) {
    var cat = AcademicCalendar.getClassify(item);
    switch (cat) {
      case 'holiday':       return '<span class="ac-month-day__dot ac-month-day__dot--holiday" title="Holiday"></span>';
      case 'exam':          return '<span class="ac-month-day__dot ac-month-day__dot--exam" title="Exam"></span>';
      case 'website_event': return '<span class="ac-month-day__dot ac-month-day__dot--webevent" title="Website Event"></span>';
      case 'academic':      return '<span class="ac-month-day__dot ac-month-day__dot--event" title="Academic Event"></span>';
      case 'working':       return '<span class="ac-month-day__dot ac-month-day__dot--working" title="Working Day"></span>';
      default:              return '<span class="ac-month-day__dot ac-month-day__dot--working" title="Working Day"></span>';
    }
  };

  /* ── Popup HTML ────────────────────────────────────────── */
  AcademicCalendar.popupHtml = function (item) {
    var ti = AcademicCalendar.getTypeInfo(item);
    var venueHtml = item.venue ? '<div class="ac-popup__desc" style="margin-top:4px;font-size:11px"><i class="bi bi-geo-alt"></i> ' + item.venue + '</div>' : '';
    var dateDisplay = item.date ? AcademicCalendar.formatDisplay(new Date(item.date + 'T00:00:00')) : '';
    var dayName = item.date ? AcademicCalendar.dayNames[new Date(item.date + 'T00:00:00').getDay()] : '';
    return '<div class="ac-popup" style="pointer-events:auto">' +
      '<div class="ac-popup__title">' + (item.title || 'Untitled') + '</div>' +
      (dateDisplay ? '<div class="ac-popup__date">' + dateDisplay + ' (' + dayName + ')</div>' : '') +
      (item.description ? '<div class="ac-popup__desc">' + item.description + '</div>' : '') +
      venueHtml +
      '<span class="ac-popup__badge ac-popup__badge--' + ti.css + '">' + (ti.shortLabel || ti.label) + '</span>' +
      '</div>';
  };

  /* ── Agenda card HTML ──────────────────────────────────── */
  AcademicCalendar.agendaCardHtml = function (item, index) {
    var hasRange = item.startDate && item.endDate;
    var ti = AcademicCalendar.getTypeInfoFromAgenda(item);

    var metaHtml = '';

    if (hasRange) {
      metaHtml += '<span class="ac-agenda-card__meta-item"><i class="bi bi-calendar-range"></i> ' + item.startDate + ' – ' + item.endDate + '</span>';
      if (item.totalSubjects) {
        metaHtml += '<span class="ac-agenda-card__meta-item"><i class="bi bi-book"></i> ' + item.totalSubjects + ' Subject' + (item.totalSubjects > 1 ? 's' : '') + '</span>';
      }
      if (item.classes && item.classes.length > 0) {
        metaHtml += '<span class="ac-agenda-card__meta-item"><i class="bi bi-people"></i> Classes: ' + item.classes.join(', ') + '</span>';
      }
    } else {
      var d = item.date ? new Date(item.date + 'T00:00:00') : new Date();
      metaHtml += '<span class="ac-agenda-card__meta-item"><i class="bi bi-calendar3"></i> ' + d.toLocaleDateString(undefined, { weekday: 'long' }) + '</span>';
      if (item.holidayType) {
        metaHtml += '<span class="ac-agenda-card__meta-item"><i class="bi bi-tag"></i> ' + item.holidayType + '</span>';
      }
      if (item.venue) {
        metaHtml += '<span class="ac-agenda-card__meta-item"><i class="bi bi-geo-alt"></i> ' + item.venue + '</span>';
      }
    }

    var dateHtml = '';
    if (hasRange) {
      dateHtml = '<div class="ac-agenda-card__date">' +
        '<span class="ac-agenda-card__date-day">' + item.startDate.split(' ')[0] + '</span>' +
        '<span class="ac-agenda-card__date-month">' + (item.startDate.split(' ')[1] || '').substring(0, 3) + '</span>' +
        '<span class="ac-agenda-card__date-year">' + (item.startDate.split(' ')[2] || '') + '</span>' +
        '</div>';
    } else {
      var dd = item.date ? new Date(item.date + 'T00:00:00') : new Date();
      dateHtml = '<div class="ac-agenda-card__date">' +
        '<span class="ac-agenda-card__date-day">' + String(dd.getDate()).padStart(2, '0') + '</span>' +
        '<span class="ac-agenda-card__date-month">' + AcademicCalendar.monthNames[dd.getMonth()].substring(0, 3) + '</span>' +
        '<span class="ac-agenda-card__date-year">' + dd.getFullYear() + '</span>' +
        '</div>';
    }

    return '<div class="ac-agenda-card" data-index="' + index + '">' +
      dateHtml +
      '<div class="ac-agenda-card__body">' +
      '<div class="ac-agenda-card__top">' +
      '<div class="ac-agenda-card__title">' + (item.title || 'Untitled') + '</div>' +
      '<span class="ac-agenda-card__badge ac-agenda-card__badge--' + ti.css + '">' + (ti.shortLabel || ti.label) + '</span>' +
      '</div>' +
      (item.description ? '<div class="ac-agenda-card__desc">' + item.description + '</div>' : '') +
      '<div class="ac-agenda-card__meta">' + metaHtml + '</div>' +
      '</div>' +
      '</div>';
  };

  /* ── Agenda section builder ────────────────────────────── */
  AcademicCalendar.renderAgendaSections = function (items, containerId) {
    var container = document.getElementById(containerId);
    if (!container) return;

    // FIX 2: Filter out working days
    var filtered = items.filter(function (item) {
      return !AcademicCalendar.isWorkingDay(item);
    });

    if (!filtered || filtered.length === 0) {
      container.innerHTML = '<div class="ac-agenda-empty"><div class="ac-agenda-empty__icon"><i class="bi bi-calendar-x"></i></div><div class="fw-bold fs-6" style="color:var(--adm-text-2)">No upcoming items</div><div class="small">Try adjusting your filters or date range.</div></div>';
      return;
    }

    var today = new Date();
    today.setHours(0, 0, 0, 0);
    var tomorrow = new Date(today);
    tomorrow.setDate(tomorrow.getDate() + 1);
    var weekFromNow = new Date(today);
    weekFromNow.setDate(weekFromNow.getDate() + 7);

    var groups = {
      today: { label: 'Today', icon: 'bi-sun', items: [] },
      tomorrow: { label: 'Tomorrow', icon: 'bi-sunrise', items: [] },
      thisWeek: { label: 'This Week', icon: 'bi-calendar-week', items: [] },
      upcoming: { label: 'Upcoming', icon: 'bi-calendar3', items: [] }
    };

    filtered.forEach(function (item, idx) {
      var d;
      if (item.startDate) {
        d = new Date(item.startDate + 'T00:00:00');
      } else if (item.date) {
        d = new Date(item.date + 'T00:00:00');
      } else {
        return;
      }
      if (d.getTime() === today.getTime()) {
        groups.today.items.push(idx);
      } else if (d.getTime() === tomorrow.getTime()) {
        groups.tomorrow.items.push(idx);
      } else if (d > today && d <= weekFromNow) {
        groups.thisWeek.items.push(idx);
      } else {
        groups.upcoming.items.push(idx);
      }
    });

    var html = '';
    Object.keys(groups).forEach(function (key) {
      var g = groups[key];
      if (g.items.length === 0) return;
      html += '<div class="ac-agenda-section">' +
        '<div class="ac-agenda-section__header">' +
        '<i class="bi ' + g.icon + '" style="font-size:18px;color:var(--adm-primary)"></i>' +
        '<h5 class="ac-agenda-section__title">' + g.label + '</h5>' +
        '<span class="ac-agenda-section__count">' + g.items.length + ' item' + (g.items.length > 1 ? 's' : '') + '</span>' +
        '</div>' +
        '<div class="ac-agenda-list">';
      g.items.forEach(function (idx) { html += AcademicCalendar.agendaCardHtml(filtered[idx], idx); });
      html += '</div></div>';
    });

    container.innerHTML = html || '<div class="ac-agenda-empty"><div class="ac-agenda-empty__icon"><i class="bi bi-calendar-x"></i></div><div class="fw-bold fs-6" style="color:var(--adm-text-2)">No items in this period</div></div>';

    // Attach click handlers for modal
    container.querySelectorAll('.ac-agenda-card').forEach(function (card, idx) {
      card.addEventListener('click', function () {
        var index = parseInt(card.getAttribute('data-index'));
        if (!isNaN(index) && filtered[index]) {
          AcademicCalendar.showEventModal(filtered[index]);
        }
      });
    });
  };

  /* ── Event Modal (single event — kept for agenda/upcoming use) ── */
  AcademicCalendar.showEventModal = function (item) {
    var items = Array.isArray(item) ? item : [item];
    if (items.length === 0) return;

    // Build event cards
    var bodyHtml = '';
    items.forEach(function (ev) {
      var ti = AcademicCalendar.getTypeInfo(ev);
      var d = ev.date ? new Date(ev.date + 'T00:00:00') : null;
      var dateStr = d ? d.toLocaleDateString(undefined, { weekday: 'long', day: 'numeric', month: 'short', year: 'numeric' }) : '';

      var meta = [];
      if (ev.startDate && ev.endDate) meta.push('<span class="badge bg-light text-dark me-1"><i class="bi bi-calendar-range me-1"></i>' + ev.startDate + ' – ' + ev.endDate + '</span>');
      if (ev.totalSubjects) meta.push('<span class="badge bg-light text-dark me-1"><i class="bi bi-book me-1"></i>' + ev.totalSubjects + ' subjects</span>');
      if (ev.venue) meta.push('<span class="badge bg-light text-dark me-1"><i class="bi bi-geo-alt me-1"></i>' + AcademicCalendar.escapeHtml(ev.venue) + '</span>');
      if (ev.holidayType) meta.push('<span class="badge bg-light text-dark me-1"><i class="bi bi-tag me-1"></i>' + AcademicCalendar.escapeHtml(ev.holidayType) + '</span>');

      bodyHtml += '<div class="card border mb-2" style="border-radius:10px;">' +
        '<div class="card-body p-3">' +
        '<div class="d-flex align-items-start justify-content-between gap-2">' +
        '<h6 class="fw-bold mb-1" style="color:var(--adm-text)">' + AcademicCalendar.escapeHtml(ev.title || 'Event') + '</h6>' +
        '<span class="badge rounded-pill px-2 py-1 flex-shrink-0" style="background:' + ti.color + '20;color:' + ti.color + ';font-weight:700;font-size:10px;">' + (ti.shortLabel || ti.label) + '</span>' +
        '</div>' +
        (ev.description ? '<p class="small text-muted mb-2">' + AcademicCalendar.escapeHtml(ev.description) + '</p>' : '') +
        (meta.length > 0 ? '<div class="d-flex flex-wrap gap-1 mt-1">' + meta.join('') + '</div>' : '') +
        '</div></div>';
    });

    var firstTi = AcademicCalendar.getTypeInfo(items[0]);
    var headerColor = firstTi.color;

    var modalHtml = [
      '<div class="modal fade" id="acEventModal" tabindex="-1" aria-hidden="true">',
      '  <div class="modal-dialog modal-dialog-centered">',
      '    <div class="modal-content border-0 shadow-lg" style="border-radius:16px;overflow:hidden;">',
      '      <div class="modal-header border-0" style="background:' + headerColor + ';color:#fff;padding:20px 24px;">',
      '        <h5 class="modal-title fw-bold"><i class="bi bi-calendar-event me-2"></i>' + AcademicCalendar.escapeHtml(items[0].title || 'Day Details') + '</h5>',
      '        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>',
      '      </div>',
      '      <div class="modal-body p-4">',
      '        <div class="d-flex align-items-center gap-2 mb-3 pb-2 border-bottom">',
      '          <i class="bi bi-calendar3 text-primary"></i>',
      '          <span class="fw-semibold">' + AcademicCalendar.escapeHtml(items[0].date || '') + '</span>',
      '          <span class="badge bg-secondary rounded-pill">' + items.length + ' event' + (items.length > 1 ? 's' : '') + '</span>',
      '        </div>',
                    bodyHtml,
      '      </div>',
      '      <div class="modal-footer border-0 pt-0 px-4 pb-4">',
      '        <button type="button" class="btn btn-outline-secondary px-4 rounded-pill" data-bs-dismiss="modal"><i class="bi bi-x-lg me-2"></i>Close</button>',
      '      </div>',
      '    </div>',
      '  </div>',
      '</div>'
    ].join('\n');

    var existing = document.getElementById('acEventModal');
    if (existing) existing.remove();

    var div = document.createElement('div');
    div.innerHTML = modalHtml;
    document.body.appendChild(div);

    var modal = new bootstrap.Modal(document.getElementById('acEventModal'));
    modal.show();

    document.getElementById('acEventModal').addEventListener('hidden.bs.modal', function () {
      div.remove();
    });
  };

  /* ═══════════════════════════════════════════════════════════════
     FIX 1 — CORRECT KPI CALCULATIONS
     ═══════════════════════════════════════════════════════════════ */
  AcademicCalendar.loadKpiCards = function (containerId) {
    var container = document.getElementById(containerId);
    if (!container) return;

    var skeletons = '';
    for (var i = 0; i < 5; i++) {
      skeletons += '<div class="ac-stat-skeleton"></div>';
    }
    container.innerHTML = skeletons;

    var year = new Date().getFullYear();
    fetch('/AcademicCalendar/GetYearData?year=' + year)
      .then(function (r) { return r.json(); })
      .then(function (data) {
        if (!data || !data.months) {
          container.innerHTML = '';
          return;
        }

        var totalWorking = 0, totalHolidays = 0, totalExams = 0, totalAcEvents = 0, totalWebEvents = 0;

        data.months.forEach(function (m) {
          totalWorking += m.workingDays || 0;
          totalHolidays += m.holidays || 0;
          totalExams += m.examDays || 0;
          totalAcEvents += m.events || 0;
          totalWebEvents += m.websiteEvents || 0;
        });

        var totalEvents = totalAcEvents + totalWebEvents;

        var cards = [
          { icon: 'bi-calendar-check', css: 'working', label: 'Total Events', value: totalEvents },
          { icon: 'bi-pencil-square', css: 'exam', label: 'Exams', value: totalExams },
          { icon: 'bi-snow', css: 'holiday', label: 'Holidays', value: totalHolidays },
          { icon: 'bi-globe', css: 'webevent', label: 'Website Events', value: totalWebEvents },
          { icon: 'bi-star', css: 'event', label: 'Academic Events', value: totalAcEvents }
        ];

        var html = '';
        cards.forEach(function (c) {
          html += '<div class="ac-stat">' +
            '<div class="ac-stat__icon ac-stat__icon--' + c.css + '"><i class="bi ' + c.icon + '"></i></div>' +
            '<div class="ac-stat__body"><div class="ac-stat__label">' + c.label + '</div><div class="ac-stat__value">' + c.value + '</div></div>' +
            '</div>';
        });
        container.innerHTML = html;
      })
      .catch(function () {
        container.innerHTML = '';
      });
  };

  /* ═══════════════════════════════════════════════════════════════
     FIX 5 — MONTH VIEW EVENT DISPLAY (coloured labels, +N More)
     ═══════════════════════════════════════════════════════════════ */
  AcademicCalendar.renderMonthGrid = function (year, month, events, containerId) {
    var container = document.getElementById(containerId);
    if (!container) return;

    var firstDay = new Date(year, month, 1);
    var lastDay = new Date(year, month + 1, 0);
    var startDay = firstDay.getDay(); // 0=Sun
    var daysInMonth = lastDay.getDate();
    var daysInPrev = new Date(year, month, 0).getDate();

    var today = new Date();
    var todayStr = AcademicCalendar.formatDate(today);

    // Build lookup: date string → array of events
    var eventMap = {};
    if (events) {
      events.forEach(function (ev) {
        var key = ev.date;
        if (key) { eventMap[key] = eventMap[key] || []; eventMap[key].push(ev); }
      });
    }

    var html = '<div class="ac-month-grid">';

    // ── Header row ──
    AcademicCalendar.dayNamesShort.forEach(function (name) {
      html += '<div class="ac-month-header__cell">' + name + '</div>';
    });

    // ── Leading empty cells (previous month) ──
    for (var i = startDay - 1; i >= 0; i--) {
      var prevDay = daysInPrev - i;
      html += '<div class="ac-month-day ac-month-day--other"><div class="ac-month-day__number">' + prevDay + '</div></div>';
    }

    // ── Current month days ──
    for (var d = 1; d <= daysInMonth; d++) {
      var dateStr = year + '-' + String(month + 1).padStart(2, '0') + '-' + String(d).padStart(2, '0');
      var isToday = dateStr === todayStr;
      var cls = 'ac-month-day';
      if (isToday) cls += ' ac-month-day--today';

      html += '<div class="' + cls + '" data-date="' + dateStr + '">';
      html += '<div class="ac-month-day__number">' + d + '</div>';

      var dayEvents = (eventMap[dateStr] || []).filter(function (ev) {
        return AcademicCalendar.matchesFilter(ev);
      });

      html += '<div class="ac-month-day__events">';

      if (dayEvents.length === 0) {
        // Working day — green dot only
        html += '<span class="ac-month-day__dot ac-month-day__dot--working" title="Working Day"></span>';
      } else {
        // Unique event labels
        var seen = {};
        var labels = [];
        dayEvents.forEach(function (ev) {
          var title = ev.title || '';
          var ti = AcademicCalendar.getTypeInfo(ev);
          if (title && !seen[title]) {
            seen[title] = true;
            labels.push({ title: title, css: ti.css });
          }
        });

        var maxVisible = 2;
        for (var li = 0; li < labels.length && li < maxVisible; li++) {
          html += '<div class="ac-month-day__title ac-month-day__title--' + labels[li].css + '" title="' + AcademicCalendar.escapeHtml(labels[li].title) + '">' + AcademicCalendar.escapeHtml(labels[li].title) + '</div>';
        }
        if (labels.length > maxVisible) {
          html += '<div class="ac-month-day__title ac-month-day__title--more">+' + (labels.length - maxVisible) + ' More</div>';
        }
      }

      html += '</div>'; // .ac-month-day__events
      html += '</div>'; // .ac-month-day
    }

    // ── Trailing empty cells (next month) ──
    var totalCells = startDay + daysInMonth;
    var trailing = (7 - (totalCells % 7)) % 7;
    for (var n = 1; n <= trailing; n++) {
      html += '<div class="ac-month-day ac-month-day--other"><div class="ac-month-day__number">' + n + '</div></div>';
    }

    html += '</div>'; // .ac-month-grid
    container.innerHTML = html;

    AcademicCalendar.attachPopups(container, events);
    AcademicCalendar.attachDayClick(container, events);
  };

  /* ── Escaping helper ──────────────────────────────────── */
  AcademicCalendar.escapeHtml = function (str) {
    if (!str) return '';
    var map = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' };
    return String(str).replace(/[&<>"']/g, function (ch) { return map[ch]; });
  };

  /* ── Day click → modal (shows ALL events for the day) ──── */
  AcademicCalendar.attachDayClick = function (container, events) {
    if (!events) return;
    var eventMap = {};
    events.forEach(function (ev) { eventMap[ev.date] = eventMap[ev.date] || []; eventMap[ev.date].push(ev); });

    var cells = container.querySelectorAll('.ac-month-day[data-date]');
    cells.forEach(function (cell) {
      cell.addEventListener('click', function (e) {
        // Ignore clicks on event labels (they have their own handlers if needed)
        if (e.target.closest('.ac-month-day__title')) return;
        var dateStr = cell.getAttribute('data-date');
        var dayEvents = eventMap[dateStr];
        if (!dayEvents || dayEvents.length === 0) return;
        var filtered = dayEvents.filter(function (ev) { return AcademicCalendar.matchesFilter(ev); });
        if (filtered.length === 0) return;
        AcademicCalendar.showEventModal(filtered);
      });
    });
  };

  /* ── Hover popup system ────────────────────────────────── */
  AcademicCalendar.attachPopups = function (container, events) {
    if (!events) return;
    var eventMap = {};
    events.forEach(function (ev) { eventMap[ev.date] = eventMap[ev.date] || []; eventMap[ev.date].push(ev); });

    var cells = container.querySelectorAll('.ac-month-day[data-date]');
    cells.forEach(function (cell) {
      var dateStr = cell.getAttribute('data-date');
      var dayEvents = eventMap[dateStr];
      if (!dayEvents) return;

      cell.addEventListener('mouseenter', function (e) {
        var existing = cell.querySelector('.ac-popup');
        if (existing) return;
        var popupContent = '';
        dayEvents.forEach(function (ev) {
          popupContent += AcademicCalendar.popupHtml(ev);
        });
        var div = document.createElement('div');
        div.innerHTML = popupContent;
        var popups = div.children;
        for (var i = 0; i < popups.length; i++) {
          cell.appendChild(popups[i]);
        }
      });
      cell.addEventListener('mouseleave', function () {
        var existing = cell.querySelectorAll('.ac-popup');
        existing.forEach(function (p) { p.remove(); });
      });
    });
  };

  /* ── Loading / Skeleton ────────────────────────────────── */
  AcademicCalendar.showLoading = function (id) {
    var el = document.getElementById(id);
    if (el) el.innerHTML = '<div class="text-center py-5"><div class="spinner-border spinner-border-sm me-2" role="status"></div>Loading...</div>';
  };

})();
