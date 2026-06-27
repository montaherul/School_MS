/* ═══════════════════════════════════════════════════
   ONBOARDING WIZARD — Enterprise HR-Style
   ═══════════════════════════════════════════════════ */

(function () {
  'use strict';

  const STORAGE_KEY_PREFIX = 'ob_';
  let currentStep = 1;
  let totalSteps = 6;
  let isDirty = false;
  let autosaveTimer = null;
  let wizardForm = null;

  // ────────── Init ──────────
  document.addEventListener('DOMContentLoaded', function () {
    wizardForm = document.getElementById('employeeOnboardingForm');
    if (!wizardForm) return;

    totalSteps = parseInt(wizardForm.dataset.steps) || 6;
    currentStep = parseInt(sessionStorage.getItem(STORAGE_KEY_PREFIX + 'step')) || 1;

    loadDraft();
    goToStep(currentStep, false);
    bindEvents();
    startAutosave();
    bindBeforeUnload();
    initPasswordMeter();
    initFileUploads();
    initCollapsibleCards();
  });

  // ────────── Navigation ──────────
  function goToStep(step, animate) {
    if (step < 1 || step > totalSteps) return;
    currentStep = step;
    sessionStorage.setItem(STORAGE_KEY_PREFIX + 'step', step);

    // Panels
    document.querySelectorAll('.wizard-step-panel').forEach(function (el, i) {
      el.classList.toggle('active', i + 1 === step);
    });

    // Sidebar nav
    document.querySelectorAll('.wizard-nav-item').forEach(function (el, i) {
      var idx = parseInt(el.dataset.step) || i + 1;
      el.classList.toggle('active', idx === step);
      if (idx < step) el.classList.add('completed');
      else el.classList.remove('completed');
    });

    // Progress dots
    document.querySelectorAll('.wizard-progress-step').forEach(function (el, i) {
      var idx = parseInt(el.dataset.step) || i + 1;
      el.classList.toggle('active', idx === step);
      el.classList.toggle('completed', idx < step);
    });

    // Progress fill
    var pct = ((step - 1) / (totalSteps - 1)) * 100;
    var fill = document.getElementById('wizardProgressFill');
    if (fill) fill.style.width = pct + '%';

    // Hero progress ring
    updateHeroProgress(pct);

    // Buttons
    var prevBtn = document.getElementById('btnPrev');
    var nextBtn = document.getElementById('btnNext');
    var submitBtn = document.getElementById('btnSubmit');
    var saveBtn = document.getElementById('btnSave');

    if (prevBtn) prevBtn.style.display = step === 1 ? 'none' : '';
    if (nextBtn) nextBtn.style.display = step === totalSteps ? 'none' : '';
    if (submitBtn) submitBtn.style.display = step === totalSteps ? '' : 'none';
    if (saveBtn) saveBtn.style.display = '';

    // Focus
    var firstInput = document.querySelector('.wizard-step-panel.active .adm-input, .wizard-step-panel.active .adm-select');
    if (firstInput) setTimeout(function () { firstInput.focus(); }, 100);

    // Scroll top
    var content = document.querySelector('.wizard-content');
    if (content) content.scrollTop = 0;

    isDirty = true;
  }

  function nextStep() {
    if (validateStep(currentStep)) {
      goToStep(currentStep + 1, true);
    }
  }

  function prevStep() {
    goToStep(currentStep - 1, true);
  }

  // ────────── Validation ──────────
  function validateStep(step) {
    var panel = document.querySelector('.wizard-step-panel:nth-child(' + step + ')');
    if (!panel) return true;

    var inputs = panel.querySelectorAll('[required]');
    var valid = true;

    inputs.forEach(function (input) {
      input.classList.remove('error');
      if (!input.value || input.value.trim() === '') {
        input.classList.add('error');
        valid = false;
      }
      // Email
      if (input.type === 'email' && input.value && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(input.value)) {
        input.classList.add('error');
        valid = false;
      }
    });

    if (!valid) {
      showToast('Please fill in all required fields before proceeding.', 'warning');
      var firstError = panel.querySelector('.error');
      if (firstError) firstError.focus();
    }

    return valid;
  }

  // ────────── Autosave ──────────
  function startAutosave() {
    if (autosaveTimer) clearInterval(autosaveTimer);
    autosaveTimer = setInterval(function () {
      if (isDirty) saveDraft();
    }, 30000);
  }

  function saveDraft() {
    var formData = new FormData(wizardForm);
    var draft = {};
    formData.forEach(function (value, key) {
      if (key.indexOf('password') === -1 && key.indexOf('file') === -1 && key.indexOf('File') === -1) {
        draft[key] = value;
      }
    });
    try {
      localStorage.setItem(STORAGE_KEY_PREFIX + 'draft', JSON.stringify(draft));
      localStorage.setItem(STORAGE_KEY_PREFIX + 'draft_ts', new Date().toISOString());
      isDirty = false;
      showSavedIndicator();
    } catch (e) {
      // Storage full - ignore
    }
  }

  function loadDraft() {
    try {
      var raw = localStorage.getItem(STORAGE_KEY_PREFIX + 'draft');
      if (!raw) return;
      var draft = JSON.parse(raw);
      Object.keys(draft).forEach(function (key) {
        var el = wizardForm.querySelector('[name="' + key + '"]');
        if (el && !el.readOnly && !el.disabled) {
          el.value = draft[key];
        }
      });
    } catch (e) { /* ignore */ }
  }

  function showSavedIndicator() {
    var badge = document.querySelector('.unsaved-badge');
    if (badge) {
      badge.innerHTML = '<i class="bi bi-check-circle-fill"></i> Saved at ' + new Date().toLocaleTimeString();
      badge.style.color = 'var(--adm-success, #059669)';
      setTimeout(function () {
        badge.innerHTML = '<i class="bi bi-exclamation-circle"></i> Unsaved changes';
        badge.style.color = '#f59e0b';
      }, 3000);
    }
  }

  // ────────── Unsaved Changes ──────────
  function bindBeforeUnload() {
    window.addEventListener('beforeunload', function (e) {
      if (isDirty) {
        e.preventDefault();
        e.returnValue = '';
      }
    });
    // Mark dirty on any input change
    wizardForm.addEventListener('input', function () { isDirty = true; });
    wizardForm.addEventListener('change', function () { isDirty = true; });
  }

  // ────────── Password Meter ──────────
  function initPasswordMeter() {
    var pwd = document.getElementById('Password');
    if (!pwd) return;

    var meterFill = document.getElementById('pwdMeterFill');
    var meterText = document.getElementById('pwdMeterText');
    var reqs = {
      length: document.getElementById('pwdReqLength'),
      upper: document.getElementById('pwdReqUpper'),
      lower: document.getElementById('pwdReqLower'),
      digit: document.getElementById('pwdReqDigit'),
      special: document.getElementById('pwdReqSpecial')
    };

    // Toggle
    var toggle = document.getElementById('pwdToggle');
    if (toggle) {
      toggle.addEventListener('click', function () {
        var type = pwd.type === 'password' ? 'text' : 'password';
        pwd.type = type;
        var icon = toggle.querySelector('i');
        if (icon) icon.className = type === 'password' ? 'bi bi-eye-slash' : 'bi bi-eye';
      });
    }

    var confirmPwd = document.getElementById('ConfirmPassword');

    pwd.addEventListener('input', function () {
      var val = pwd.value;
      var score = 0;

      // Length
      if (val.length >= 6) { score += 20; if (reqs.length) reqs.length.classList.add('met'); }
      else if (reqs.length) reqs.length.classList.remove('met');

      if (val.length >= 10) score += 10;

      // Upper
      if (/[A-Z]/.test(val)) { score += 20; if (reqs.upper) reqs.upper.classList.add('met'); }
      else if (reqs.upper) reqs.upper.classList.remove('met');

      // Lower
      if (/[a-z]/.test(val)) { score += 20; if (reqs.lower) reqs.lower.classList.add('met'); }
      else if (reqs.lower) reqs.lower.classList.remove('met');

      // Digit
      if (/\d/.test(val)) { score += 20; if (reqs.digit) reqs.digit.classList.add('met'); }
      else if (reqs.digit) reqs.digit.classList.remove('met');

      // Special
      if (/[^a-zA-Z0-9]/.test(val)) { score += 20; if (reqs.special) reqs.special.classList.add('met'); }
      else if (reqs.special) reqs.special.classList.remove('met');

      if (!meterFill || !meterText) return;

      if (val.length === 0) {
        meterFill.style.width = '0';
        meterFill.className = 'password-meter-fill';
        meterText.textContent = '';
        return;
      }

      var level = score <= 20 ? 'weak' : score <= 40 ? 'fair' : score <= 60 ? 'good' : 'strong';
      meterFill.className = 'password-meter-fill ' + level;

      var labels = { weak: 'Weak — Try adding numbers & symbols', fair: 'Fair — Add uppercase & special chars', good: 'Good — Almost there', strong: 'Strong — Great password!' };
      meterText.textContent = labels[level] || '';
    });
  }

  // ────────── File Uploads ──────────
  function initFileUploads() {
    document.querySelectorAll('.wizard-upload-zone').forEach(function (zone) {
      var input = zone.querySelector('input[type="file"]');
      if (!input) return;

      zone.addEventListener('dragover', function (e) {
        e.preventDefault();
        zone.classList.add('drag-over');
      });
      zone.addEventListener('dragleave', function () {
        zone.classList.remove('drag-over');
      });
      zone.addEventListener('drop', function (e) {
        e.preventDefault();
        zone.classList.remove('drag-over');
        if (input && e.dataTransfer.files.length) {
          input.files = e.dataTransfer.files;
          handleFileSelect(input, zone);
        }
      });
      input.addEventListener('change', function () {
        handleFileSelect(input, zone);
      });
    });
  }

  function handleFileSelect(input, zone) {
    var file = input.files && input.files[0];
    if (!file) return;

    var preview = zone.parentNode.querySelector('.wizard-upload-preview');
    if (!preview) {
      preview = document.createElement('div');
      preview.className = 'wizard-upload-preview';
      zone.parentNode.insertBefore(preview, zone.nextSibling);
    }

    if (file.type.startsWith('image/')) {
      var reader = new FileReader();
      reader.onload = function (e) {
        preview.innerHTML =
          '<img src="' + e.target.result + '" alt="Preview"/>' +
          '<div class="file-info"><div class="file-name">' + file.name + '</div><div class="file-size">' + formatSize(file.size) + '</div></div>' +
          '<button type="button" class="file-remove" title="Remove"><i class="bi bi-x-circle"></i></button>';
        bindFileRemove(preview, input, zone);
      };
      reader.readAsDataURL(file);
    } else {
      preview.innerHTML =
        '<div style="width:48px;height:48px;border-radius:6px;background:var(--adm-surface-2);display:flex;align-items:center;justify-content:center;font-size:24px"><i class="bi bi-file-earmark-text"></i></div>' +
        '<div class="file-info"><div class="file-name">' + file.name + '</div><div class="file-size">' + formatSize(file.size) + '</div></div>' +
        '<button type="button" class="file-remove" title="Remove"><i class="bi bi-x-circle"></i></button>';
      bindFileRemove(preview, input, zone);
    }
    zone.style.display = 'none';
  }

  function bindFileRemove(preview, input, zone) {
    var btn = preview.querySelector('.file-remove');
    if (btn) {
      btn.addEventListener('click', function () {
        preview.remove();
        zone.style.display = '';
        input.value = '';
      });
    }
  }

  function formatSize(bytes) {
    if (bytes < 1024) return bytes + ' B';
    if (bytes < 1048576) return (bytes / 1024).toFixed(1) + ' KB';
    return (bytes / 1048576).toFixed(1) + ' MB';
  }

  // ────────── Collapsible Cards ──────────
  function initCollapsibleCards() {
    document.querySelectorAll('.wizard-card-title').forEach(function (title) {
      title.addEventListener('click', function () {
        var body = this.nextElementSibling;
        if (body && body.classList.contains('wizard-card-body')) {
          body.classList.toggle('collapsed');
          this.classList.toggle('collapsed');
        }
      });
    });
  }

  // ────────── Review Summary ──────────
  function buildReviewSummary() {
    var container = document.getElementById('reviewSummary');
    if (!container) return;

    var html = '';
    var sections = [
      { title: 'Personal Information', fields: ['FullName', 'Gender', 'DateOfBirth', 'FatherName', 'MotherName', 'BloodGroup', 'Religion', 'Nationality', 'NIDNumber', 'BirthCertificateNo'] },
      { title: 'Contact Information', fields: ['MobileNumber', 'PersonalEmail', 'PresentAddress', 'PermanentAddress', 'EmergencyContactName', 'EmergencyContactPhone'] }
    ];

    sections.forEach(function (section) {
      html += '<div class="review-section"><div class="review-section-title"><i class="bi bi-info-circle"></i> ' + section.title + '</div><div class="review-grid">';
      section.fields.forEach(function (field) {
        var input = wizardForm.querySelector('[name="' + field + '"]');
        var val = input ? input.value || '-' : '-';
        var label = '';
        var labelEl = wizardForm.querySelector('label[for="' + field + '"], label[asp-for="' + field + '"]');
        if (labelEl) label = labelEl.textContent.replace('*', '').trim();
        else label = field.replace(/([A-Z])/g, ' $1').trim();
        html += '<div class="review-item"><span class="review-item-label">' + label + '</span><span class="review-item-value">' + val + '</span></div>';
      });
      html += '</div></div>';
    });

    container.innerHTML = html;

    // Build checklist
    var checklist = document.getElementById('reviewChecklist');
    if (checklist) {
      var checks = [
        { id: 'chkPassword', label: 'Password set and confirmed', el: wizardForm.querySelector('#Password') },
        { id: 'chkPersonal', label: 'Personal information completed', el: wizardForm.querySelector('[name="Gender"]') },
        { id: 'chkContact', label: 'Contact information completed', el: wizardForm.querySelector('[name="PresentAddress"]') },
        { id: 'chkAgreed', label: 'Terms confirmed', el: document.getElementById('chkAgreed') }
      ];
      var chkHtml = '<div class="review-checklist">';
      checks.forEach(function (c) {
        var passed = c.el && c.el.value && c.el.value.trim() !== '' && (c.id !== 'chkAgreed' || c.el.checked);
        if (c.id === 'chkAgreed') passed = c.el && c.el.checked;
        if (c.id === 'chkPassword') passed = c.el && c.el.value && c.el.value.length >= 6;
        chkHtml += '<div class="review-checklist-item ' + (passed ? 'pass' : 'fail') + '">' +
          '<i class="bi bi-' + (passed ? 'check-circle-fill' : 'x-circle-fill') + '"></i> ' + c.label +
          '</div>';
      });
      chkHtml += '</div>';
      checklist.innerHTML = chkHtml;
    }
  }

  // ────────── Hero Progress ──────────
  function updateHeroProgress(pct) {
    var val = document.querySelector('.hero-progress-ring .ring-value');
    var fill = document.querySelector('.hero-progress-ring .ring-fill');
    if (val) val.textContent = Math.round(pct) + '%';
    if (fill) fill.style.width = pct + '%';
  }

  // ────────── Toast ──────────
  function showToast(msg, type) {
    if (typeof toastr !== 'undefined') {
      toastr[type || 'info'](msg);
    } else {
      alert(msg);
    }
  }

  // ────────── Expose Globally ──────────
  window.nextStep = nextStep;
  window.prevStep = prevStep;
  window.goToStep = goToStep;
  window.saveDraft = saveDraft;
  window.buildReviewSummary = buildReviewSummary;

})();
