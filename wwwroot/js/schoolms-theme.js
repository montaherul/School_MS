(function () {
    'use strict';

    var STORAGE_KEY = 'schoolms-theme';
    var THEME_DARK = 'dark';
    var THEME_LIGHT = 'light';

    function getStoredTheme() {
        try {
            return localStorage.getItem(STORAGE_KEY);
        } catch (e) {
            return null;
        }
    }

    function setStoredTheme(theme) {
        try {
            localStorage.setItem(STORAGE_KEY, theme);
        } catch (e) {
        }
    }

    function getPreferredTheme() {
        var stored = getStoredTheme();
        if (stored) {
            return stored;
        }
        return window.matchMedia('(prefers-color-scheme: dark)').matches ? THEME_DARK : THEME_LIGHT;
    }

    function setTheme(theme) {
        document.documentElement.setAttribute('data-theme', theme);
        setStoredTheme(theme);

        var toggles = document.querySelectorAll('.sms-theme-toggle');
        toggles.forEach(function (btn) {
            var icon = btn.querySelector('i');
            if (icon) {
                if (theme === THEME_DARK) {
                    icon.className = 'fas fa-sun';
                } else {
                    icon.className = 'fas fa-moon';
                }
            }
        });
    }

    function toggleTheme() {
        var current = document.documentElement.getAttribute('data-theme') || THEME_LIGHT;
        var next = current === THEME_DARK ? THEME_LIGHT : THEME_DARK;
        setTheme(next);
    }

    document.addEventListener('DOMContentLoaded', function () {
        setTheme(getPreferredTheme());

        document.addEventListener('click', function (e) {
            var btn = e.target.closest('.sms-theme-toggle');
            if (btn) {
                toggleTheme();
            }
        });
    });

    window.schoolmsTheme = {
        setTheme: setTheme,
        toggleTheme: toggleTheme,
        getTheme: function () { return document.documentElement.getAttribute('data-theme') || THEME_LIGHT; }
    };
})();
