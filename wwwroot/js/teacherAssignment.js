window.TeacherAssignmentCascade = {
    // Utility to determine if a class is group-based using data attribute
    isGroupBasedClass: function(classSelectEl) {
        if (!classSelectEl || !classSelectEl.length) return false;
        const selectedOpt = classSelectEl.find('option:selected');
        return selectedOpt.attr('data-is-group-based') === 'true';
    },

    // Main setup function to wire up selectors
    init: function(options) {
        const defaults = {
            teacherId: null,
            classSelect: null,
            groupSelect: null,
            groupContainer: null,
            sectionSelect: null,
            subjectSelect: null,
            onSectionLoaded: null,
            onSubjectLoaded: null,
            onChanged: null
        };

        const settings = Object.assign({}, defaults, options);
        if (!settings.classSelect) return;

        const classEl = $(settings.classSelect);
        const groupEl = settings.groupSelect ? $(settings.groupSelect) : null;
        const groupCont = settings.groupContainer ? $(settings.groupContainer) : null;
        const sectionEl = settings.sectionSelect ? $(settings.sectionSelect) : null;
        const subjectEl = settings.subjectSelect ? $(settings.subjectSelect) : null;

        // Function to reset a select control
        function resetSelect(el, defaultText) {
            if (!el || el.length === 0) return;
            el.empty().append(`<option value="">${defaultText}</option>`).prop('disabled', true);
        }

        // 1. Class Selection Change
        classEl.change(async function() {
            const classId = $(this).val();
            
            // Reset cascading elements
            if (groupEl) resetSelect(groupEl, groupEl.data('placeholder') || '-- Choose Group --');
            if (groupCont) groupCont.addClass('d-none');
            if (sectionEl) resetSelect(sectionEl, sectionEl.data('placeholder') || '-- Choose Section --');
            if (subjectEl) resetSelect(subjectEl, subjectEl.data('placeholder') || '-- Choose Subject --');

            if (!classId) {
                if (settings.onChanged) settings.onChanged();
                return;
            }

            const isSec = window.TeacherAssignmentCascade.isGroupBasedClass(classEl);

            if (isSec && groupEl) {
                // Fetch and populate Groups
                try {
                    const url = settings.teacherId 
                        ? `/TeacherAssignment/GetAssignedGroups/${settings.teacherId}/${classId}`
                        : `/StudentAttendance/GetGroups?classId=${classId}`; // Reusing public/attendance endpoint
                    
                    const response = await fetch(url);
                    const responseData = await response.json();
                    
                    // For student attendance/admin endpoint, data is in .data property, for teacher assignment it is a flat list
                    const groups = Array.isArray(responseData) ? responseData : (responseData.data || []);
                    
                    if (groups && groups.length > 0) {
                        groupEl.empty().append(`<option value="">${groupEl.data('placeholder') || '-- Choose Group --'}</option>`);
                        groups.forEach(g => {
                            groupEl.append($('<option>', { value: g.id || g.groupId, text: g.name || g.groupName }));
                        });
                        groupEl.prop('disabled', false);
                        if (groupCont) groupCont.removeClass('d-none');
                    } else {
                        // If secondary but no groups are returned, load sections directly
                        await loadSections(classId, null);
                    }
                } catch (e) {
                    console.error("Error loading groups:", e);
                    await loadSections(classId, null);
                }
            } else {
                // Non-secondary, load sections directly
                await loadSections(classId, null);
            }
            if (settings.onChanged) settings.onChanged();
        });

        // 2. Group Selection Change
        if (groupEl) {
            groupEl.change(async function() {
                const classId = classEl.val();
                const groupId = $(this).val();
                
                if (sectionEl) resetSelect(sectionEl, sectionEl.data('placeholder') || '-- Choose Section --');
                if (subjectEl) resetSelect(subjectEl, subjectEl.data('placeholder') || '-- Choose Subject --');

                if (classId) {
                    await loadSections(classId, groupId);
                }
                if (settings.onChanged) settings.onChanged();
            });
        }

        // 3. Section Selection Change
        if (sectionEl) {
            sectionEl.change(async function() {
                const classId = classEl.val();
                const groupId = groupEl ? groupEl.val() : null;
                const sectionId = $(this).val();

                if (subjectEl) resetSelect(subjectEl, subjectEl.data('placeholder') || '-- Choose Subject --');

                if (classId && sectionId) {
                    await loadSubjects(classId, groupId, sectionId);
                }
                if (settings.onChanged) settings.onChanged();
            });
        }

        if (subjectEl) {
            subjectEl.change(function() {
                if (settings.onChanged) settings.onChanged();
            });
        }

        // Helper: Fetch and load sections
        async function loadSections(classId, groupId) {
            if (!sectionEl || sectionEl.length === 0) return;
            try {
                let url;
                if (settings.teacherId) {
                    url = `/TeacherAssignment/GetAssignedSections/${settings.teacherId}/${classId}`;
                    if (groupId) {
                        url += `?groupId=${groupId}`;
                    }
                } else {
                    // Admin/General fallback: SectionController
                    url = `/Section/GetSectionsByClass?classId=${classId}`;
                    if (groupId) {
                        url += `&groupId=${groupId}`;
                    }
                }

                const response = await fetch(url);
                const responseData = await response.json();
                
                // Handle different JSON formats if any
                const sections = Array.isArray(responseData) ? responseData : (responseData.data || []);
                
                sectionEl.empty().append(`<option value="">${sectionEl.data('placeholder') || '-- Choose Section --'}</option>`);
                if (sections && sections.length > 0) {
                    sections.forEach(s => {
                        sectionEl.append($('<option>', { value: s.id, text: s.name }));
                    });
                    sectionEl.prop('disabled', false);
                }
                if (settings.onSectionLoaded) settings.onSectionLoaded(sections);
            } catch (e) {
                console.error("Error loading sections:", e);
            }
        }

        // Helper: Fetch and load subjects
        async function loadSubjects(classId, groupId, sectionId) {
            if (!subjectEl || subjectEl.length === 0) return;
            try {
                let url;
                if (settings.teacherId) {
                    url = `/TeacherAssignment/GetAssignedSubjects/${settings.teacherId}/${classId}?sectionId=${sectionId}`;
                    if (groupId) {
                        url += `&groupId=${groupId}`;
                    }
                } else {
                    // Admin/General fallback
                    url = `/ResultManagement/GetSubjectsForClass?classId=${classId}&sectionId=${sectionId}`;
                    if (groupId) {
                        url += `&groupId=${groupId}`;
                    }
                }

                const response = await fetch(url);
                const responseData = await response.json();
                
                const subjects = Array.isArray(responseData) ? responseData : (responseData.data || []);
                
                subjectEl.empty().append(`<option value="">${subjectEl.data('placeholder') || '-- Choose Subject --'}</option>`);
                if (subjects && subjects.length > 0) {
                    subjects.forEach(s => {
                        const id = s.id !== undefined ? s.id : s.subjectId;
                        const name = s.name !== undefined ? s.name : s.subjectName;
                        subjectEl.append($('<option>', { value: id, text: name }));
                    });
                    subjectEl.prop('disabled', false);
                }
                if (settings.onSubjectLoaded) settings.onSubjectLoaded(subjects);
            } catch (e) {
                console.error("Error loading subjects:", e);
            }
        }
    }
};
