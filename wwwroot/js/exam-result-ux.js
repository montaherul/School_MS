(function (window) {
    'use strict';

    function ensureToastContainer() {
        var id = 'examResultToastContainer';
        var el = document.getElementById(id);
        if (!el) {
            el = document.createElement('div');
            el.id = id;
            el.className = 'toast-container position-fixed bottom-0 end-0 p-3';
            el.style.zIndex = '1080';
            document.body.appendChild(el);
        }
        return el;
    }

    window.showExamToast = function (message, type) {
        type = type || 'info';
        var container = ensureToastContainer();
        var bg = type === 'success' ? 'text-bg-success'
            : type === 'error' ? 'text-bg-danger'
            : type === 'warning' ? 'text-bg-warning'
            : 'text-bg-primary';
        var toastEl = document.createElement('div');
        toastEl.className = 'toast align-items-center border-0 ' + bg;
        toastEl.setAttribute('role', 'alert');
        toastEl.innerHTML = '<div class="d-flex"><div class="toast-body">' + message + '</div>'
            + '<button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button></div>';
        container.appendChild(toastEl);
        if (window.bootstrap && bootstrap.Toast) {
            var t = new bootstrap.Toast(toastEl, { delay: 4000 });
            toastEl.addEventListener('hidden.bs.toast', function () { toastEl.remove(); });
            t.show();
        } else {
            toastEl.classList.add('show');
            setTimeout(function () { toastEl.remove(); }, 4000);
        }
    };

    window.setExamLoading = function (visible, text) {
        var overlay = document.getElementById('examResultLoadingOverlay');
        if (!overlay) {
            overlay = document.createElement('div');
            overlay.id = 'examResultLoadingOverlay';
            overlay.className = 'adm-loading adm-loading--hidden';
            overlay.innerHTML = '<div class="adm-loading__spinner"></div><div class="adm-loading__text">Loading…</div>';
            document.body.appendChild(overlay);
        }
        var label = overlay.querySelector('.adm-loading__text');
        if (label && text) label.textContent = text;
        overlay.classList.toggle('adm-loading--hidden', !visible);
        overlay.classList.toggle('adm-loading--visible', !!visible);
    };

    window.withExamButtonLoading = function (btn, promise) {
        if (!btn) return promise;
        var orig = btn.innerHTML;
        btn.disabled = true;
        btn.classList.add('pe-none');
        if (!btn.querySelector('.spinner-border')) {
            btn.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span>' + (btn.textContent || '').trim();
        }
        return Promise.resolve(promise).finally(function () {
            btn.disabled = false;
            btn.classList.remove('pe-none');
            btn.innerHTML = orig;
        });
    };
})(window);
