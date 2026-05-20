const jokesGrid = document.getElementById("jokesGrid");
const loading = document.getElementById("loading");
const jokeTitle = document.getElementById("jokeTitle");
const jokeContent = document.getElementById("jokeContent");
const jokeCategory = document.getElementById("jokeCategory");
const jokeRating = document.getElementById("jokeRating");
const jokeImageUrl = document.getElementById("jokeImageUrl");
const addBtn = document.getElementById("addBtn");
const errorMessage = document.getElementById("errorMessage");
const jokesCount = document.getElementById("jokesCount");
const showArchived = document.getElementById("showArchived");

function formatDate(dateString) {
    if (!dateString) return "неизвестно";
    return new Date(dateString).toLocaleDateString("ru-RU", {
        day: "numeric",
        month: "long",
        year: "numeric"
    });
}

function getStars(rating) {
    return "⭐".repeat(rating) + "☆".repeat(5 - rating);
}

function escapeHtml(text) {
    if (!text) return "";
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

function createCardHTML(joke) {
    const stars = getStars(joke.rating);
    const imageUrl = joke.imageUrl || `https://picsum.photos/id/${(joke.id * 7) % 200 + 1}/200/150`;
    const isPinned = joke.isPinned;
    const isArchived = joke.isArchived;
    
    return `
        <div class="joke-card ${isPinned ? 'pinned' : ''} ${isArchived ? 'archived' : ''}">
            <img class="joke-image" src="${imageUrl}" alt="Фото к анекдоту" loading="lazy" onerror="this.src='https://picsum.photos/id/1/200/150'">
            <div class="joke-content">
                <div class="joke-category" style="background-color: ${joke.categoryColor || '#667eea'}">
                    📂 ${escapeHtml(joke.categoryName)}
                </div>
                <div class="joke-title">${escapeHtml(joke.title)}</div>
                ${joke.content ? `<div class="joke-content-text">${escapeHtml(joke.content)}</div>` : ''}
                <div class="joke-rating">${stars}</div>
                <div class="joke-date">📅 ${formatDate(joke.createdAt)}</div>
                <div class="joke-actions">
                    <button class="pin-btn ${isPinned ? 'active' : ''}" data-id="${joke.id}">
                        📌 ${isPinned ? 'Открепить' : 'Закрепить'}
                    </button>
                    <button class="archive-btn" data-id="${joke.id}">
                        🗄 ${isArchived ? 'Восстановить' : 'Архивировать'}
                    </button>
                    <button class="delete-btn" data-id="${joke.id}">
                        🗑 Удалить
                    </button>
                </div>
            </div>
        </div>
    `;
}

function renderJokes(jokes) {
    loading.style.display = "none";
    jokesCount.textContent = `Всего анекдотов: ${jokes.length}`;
    
    if (jokes.length === 0) {
        jokesGrid.innerHTML = '<p class="empty-text">😢 Анекдотов пока нет. Добавьте первый!</p>';
        return;
    }
    
    jokesGrid.innerHTML = jokes.map(joke => createCardHTML(joke)).join("");
    
    document.querySelectorAll('.pin-btn').forEach(btn => {
        btn.addEventListener('click', async () => {
            const id = parseInt(btn.dataset.id);
            await handlePin(id);
        });
    });
    
    document.querySelectorAll('.archive-btn').forEach(btn => {
        btn.addEventListener('click', async () => {
            const id = parseInt(btn.dataset.id);
            await handleArchive(id);
        });
    });
    
    document.querySelectorAll('.delete-btn').forEach(btn => {
        btn.addEventListener('click', async () => {
            const id = parseInt(btn.dataset.id);
            await handleDelete(id);
        });
    });
}

async function loadJokes() {
    loading.style.display = "block";
    try {
        const archived = showArchived.checked;
        const jokes = await getAllJokes(archived);
        renderJokes(jokes);
    } catch (error) {
        loading.style.display = "none";
        jokesGrid.innerHTML = `<p class="empty-text">❌ Ошибка: ${error.message}</p>`;
        console.error(error);
    }
}

async function handleAdd() {
    const title = jokeTitle.value.trim();
    const content = jokeContent.value.trim();
    const categoryId = jokeCategory.value;
    const rating = parseInt(jokeRating.value);
    const imageUrl = jokeImageUrl.value.trim();
    
    errorMessage.textContent = "";
    
    if (!title) {
        errorMessage.textContent = "Введите текст анекдота";
        jokeTitle.focus();
        return;
    }
    
    addBtn.disabled = true;
    addBtn.textContent = "Добавляем...";
    
    try {
        await addJoke(title, content, categoryId, rating, imageUrl);
        jokeTitle.value = "";
        jokeContent.value = "";
        jokeImageUrl.value = "";
        await loadJokes();
    } catch (error) {
        errorMessage.textContent = error.message;
    } finally {
        addBtn.disabled = false;
        addBtn.textContent = "➕ Добавить анекдот";
    }
}

async function handlePin(id) {
    try {
        await togglePin(id);
        await loadJokes();
    } catch (error) {
        alert("Ошибка: " + error.message);
    }
}

async function handleArchive(id) {
    try {
        await toggleArchive(id);
        await loadJokes();
    } catch (error) {
        alert("Ошибка: " + error.message);
    }
}

async function handleDelete(id) {
    if (!confirm("🗑 Удалить этот анекдот?")) return;
    
    try {
        await deleteJoke(id);
        await loadJokes();
    } catch (error) {
        alert("Ошибка при удалении: " + error.message);
    }
}

addBtn.addEventListener("click", handleAdd);
jokeTitle.addEventListener("keydown", (e) => {
    if (e.key === "Enter") handleAdd();
});
showArchived.addEventListener("change", () => loadJokes());

function createFlyingEmojis() {
    const emojis = ['😂', '🤣', '😅', '🤪', '😎', '🔥', '💀', '✨', '🎉', '🍿', '💩', '🐸', '🍕', '🎈', '💖', '🤡', '🎪', '🃏', '⭐', '🌈', '🍺', '🎸', '💃', '🕺', '🐧', '🍦'];
    const container = document.createElement('div');
    container.className = 'flying-emojis';
    document.body.appendChild(container);
    
    for (let i = 0; i < 50; i++) {
        const emoji = document.createElement('span');
        emoji.textContent = emojis[Math.floor(Math.random() * emojis.length)];
        
        const startX = Math.random() * window.innerWidth;
        const startY = Math.random() * window.innerHeight;
        const tx = (Math.random() - 0.5) * 600;
        const ty = (Math.random() - 0.5) * 600;
        const duration = 5 + Math.random() * 10;
        
        emoji.style.left = startX + 'px';
        emoji.style.top = startY + 'px';
        emoji.style.fontSize = (20 + Math.random() * 45) + 'px';
        emoji.style.opacity = 0.2 + Math.random() * 0.3;
        emoji.style.setProperty('--tx', tx + 'px');
        emoji.style.setProperty('--ty', ty + 'px');
        emoji.style.animation = `flyRandom ${duration}s linear infinite`;
        
        container.appendChild(emoji);
    }
}

createFlyingEmojis();
loadJokes();