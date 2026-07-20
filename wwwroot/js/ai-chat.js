// AI Chat Module — Enterprise JavaScript
(function () {
    'use strict';

    const messageInput = document.getElementById('message-input');
    const sendBtn = document.getElementById('send-btn');
    const messagesContainer = document.getElementById('messages-container');
    const typingIndicator = document.getElementById('typing-indicator');
    const conversationList = document.getElementById('conversation-list');

    // ── Auto-resize textarea ──
    if (messageInput) {
        messageInput.addEventListener('input', function () {
            this.style.height = 'auto';
            this.style.height = Math.min(this.scrollHeight, 150) + 'px';
            sendBtn.disabled = this.value.trim().length === 0;
        });

        messageInput.addEventListener('keydown', function (e) {
            if (e.key === 'Enter' && !e.shiftKey) {
                e.preventDefault();
                sendMessage();
            }
        });
    }

    // ── Send Message ──
    if (sendBtn) {
        sendBtn.addEventListener('click', sendMessage);
    }

    function sendMessage() {
        if (!messageInput || !sendBtn) return;

        const message = messageInput.value.trim();
        if (!message) return;

        const conversationId = messageInput.dataset.conversationId;
        if (!conversationId) return;

        // Add user message to UI
        addMessage('user', message);
        messageInput.value = '';
        messageInput.style.height = 'auto';
        sendBtn.disabled = true;

        // Show typing indicator
        if (typingIndicator) typingIndicator.style.display = 'flex';

        // Scroll to bottom
        scrollToBottom();

        // Send to server
        const formData = new FormData();
        formData.append('conversationId', conversationId);
        formData.append('message', message);

        fetch('/AI/AIChat/Send', {
            method: 'POST',
            body: formData,
            headers: {
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
            }
        })
        .then(response => response.json())
        .then(data => {
            if (typingIndicator) typingIndicator.style.display = 'none';

            if (data.success) {
                addMessage('assistant', data.content);
                scrollToBottom();
            } else {
                addMessage('assistant', '**Error:** ' + (data.error || 'Something went wrong. Please try again.'));
                scrollToBottom();
            }
        })
        .catch(() => {
            if (typingIndicator) typingIndicator.style.display = 'none';
            addMessage('assistant', '**Error:** Network error. Please check your connection and try again.');
            scrollToBottom();
        });
    }

    // ── Add Message to UI ──
    function addMessage(role, content) {
        if (!messagesContainer) return;

        // Remove welcome message if present
        const welcome = messagesContainer.querySelector('.ai-welcome-msg');
        if (welcome) welcome.remove();

        const isUser = role === 'user';
        const div = document.createElement('div');
        div.className = 'ai-message ' + (isUser ? 'ai-user-msg' : 'ai-assistant-msg');
        div.dataset.role = role;

        div.innerHTML = `
            <div class="ai-avatar">
                <i class="bi ${isUser ? 'bi-person-circle' : 'bi-robot'}"></i>
            </div>
            <div class="ai-bubble">
                <div class="ai-msg-header">
                    <span class="ai-msg-role">${isUser ? 'You' : 'AI Assistant'}</span>
                    <span class="ai-msg-time">${new Date().toLocaleTimeString([], { hour: 'numeric', minute: '2-digit' })}</span>
                </div>
                <div class="ai-msg-content markdown-body">
                    ${isUser ? escapeHtml(content).replace(/\n/g, '<br/>') : renderMarkdown(content)}
                </div>
                ${!isUser ? `
                <div class="ai-msg-actions">
                    <button class="ai-copy-btn" title="Copy"><i class="bi bi-clipboard"></i></button>
                    <button class="ai-regenerate-btn" title="Regenerate"><i class="bi bi-arrow-clockwise"></i></button>
                </div>` : ''}
            </div>
        `;

        messagesContainer.appendChild(div);

        // Render math
        if (window.renderMathInElement) {
            try {
                renderMathInElement(div, { delimiters: [{ left: '$$', right: '$$', display: true }, { left: '$', right: '$', display: false }] });
            } catch (e) { /* KaTeX not available */ }
        }

        // Highlight code
        div.querySelectorAll('pre code').forEach(block => {
            if (window.hljs) {
                hljs.highlightElement(block);
            }
        });

        scrollToBottom();
    }

    // ── Render Markdown ──
    function renderMarkdown(text) {
        if (!text) return '';

        if (window.marked) {
            try {
                return marked.parse(text, { breaks: true, gfm: true });
            } catch (e) {
                return escapeHtml(text);
            }
        }

        // Fallback: basic markdown
        return escapeHtml(text)
            .replace(/### (.+)/g, '<h3>$1</h3>')
            .replace(/## (.+)/g, '<h2>$1</h2>')
            .replace(/# (.+)/g, '<h1>$1</h1>')
            .replace(/\*\*(.+?)\*\*/g, '<strong>$1</strong>')
            .replace(/\*(.+?)\*/g, '<em>$1</em>')
            .replace(/`([^`]+)`/g, '<code>$1</code>')
            .replace(/\n/g, '<br/>');
    }

    // ── Escape HTML ──
    function escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    // ── Scroll to Bottom ──
    function scrollToBottom() {
        if (messagesContainer) {
            messagesContainer.scrollTop = messagesContainer.scrollHeight;
        }
    }

    // ── Copy Message ──
    document.addEventListener('click', function (e) {
        const copyBtn = e.target.closest('.ai-copy-btn');
        if (copyBtn) {
            const content = copyBtn.closest('.ai-bubble')?.querySelector('.ai-msg-content');
            if (content) {
                const text = content.textContent || '';
                navigator.clipboard.writeText(text).then(() => {
                    const icon = copyBtn.querySelector('i');
                    if (icon) {
                        icon.className = 'bi bi-check-lg';
                        setTimeout(() => { icon.className = 'bi bi-clipboard'; }, 2000);
                    }
                });
            }
        }
    });

    // ── Regenerate ──
    document.addEventListener('click', function (e) {
        const regenBtn = e.target.closest('.ai-regenerate-btn');
        if (regenBtn) {
            const bubble = regenBtn.closest('.ai-bubble');
            const msgDiv = regenBtn.closest('.ai-message');
            if (msgDiv) {
                // Find the last user message
                const allMessages = messagesContainer?.querySelectorAll('.ai-message');
                if (allMessages) {
                    for (let i = allMessages.length - 1; i >= 0; i--) {
                        if (allMessages[i].classList.contains('ai-user-msg')) {
                            const userContent = allMessages[i].querySelector('.ai-msg-content');
                            if (userContent) {
                                const text = userContent.textContent || '';
                                if (messageInput) {
                                    messageInput.value = text;
                                    messageInput.dataset.conversationId = messageInput.dataset.conversationId;
                                    sendBtn.disabled = false;
                                    messageInput.focus();
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }
    });

    // ── Suggestion Chips ──
    document.addEventListener('click', function (e) {
        const chip = e.target.closest('.ai-suggestion-chip');
        if (chip) {
            const prompt = chip.dataset.prompt;
            if (messageInput) {
                messageInput.value = prompt;
                sendBtn.disabled = false;
                sendMessage();
            }
        }
    });

    // ── Load More Conversations ──
    document.addEventListener('click', function (e) {
        const loadMore = e.target.closest('.ai-load-more');
        if (loadMore) {
            const page = parseInt(loadMore.dataset.page) || 2;
            loadMore.disabled = true;
            loadMore.textContent = 'Loading...';

            fetch(`/AI/AIChat/List?page=${page}`)
                .then(response => response.json())
                .then(data => {
                    if (data.data && data.data.length > 0) {
                        const tempDiv = document.createElement('div');
                        data.data.forEach(conv => {
                            const isActive = window.location.pathname.includes('/Chat/' + conv.id);
                            const item = document.createElement('a');
                            item.href = `/AI/AIChat/Chat/${conv.id}`;
                            item.className = 'ai-conv-item' + (isActive ? ' active' : '');
                            item.dataset.id = conv.id;
                            item.innerHTML = `
                                <div class="ai-conv-icon"><i class="bi bi-chat-dots"></i></div>
                                <div class="ai-conv-info">
                                    <div class="ai-conv-title">${escapeHtml(conv.title)}</div>
                                    <div class="ai-conv-meta">${conv.messageCount} msgs</div>
                                </div>
                            `;
                            loadMore.parentNode.insertBefore(item, loadMore);
                        });

                        if (page < data.totalPages) {
                            loadMore.dataset.page = page + 1;
                            loadMore.disabled = false;
                            loadMore.textContent = 'Load more';
                        } else {
                            loadMore.remove();
                        }
                    } else {
                        loadMore.remove();
                    }
                })
                .catch(() => {
                    loadMore.disabled = false;
                    loadMore.textContent = 'Load more';
                });
        }
    });

    // ── Initial scroll to bottom if messages exist ──
    if (messagesContainer) {
        scrollToBottom();
    }

})();
