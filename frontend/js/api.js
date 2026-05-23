const API_URL = "http://localhost:5164/api/jokes";

async function getAllJokes(showArchived = false) {
  const url = showArchived ? `${API_URL}?archived=true` : API_URL;
  const response = await fetch(url);

  if (!response.ok) {
    throw new Error("Не удалось загрузить анекдоты");
  }

  const result = await response.json();
  return result.data;
}

async function addJoke(title, content, categoryId, rating, imageFile) {
  const formData = new FormData();
  formData.append("title", title);
  formData.append("content", content || "");
  formData.append("categoryId", parseInt(categoryId));
  formData.append("rating", parseInt(rating));

  if (imageFile) {
    formData.append("imageFile", imageFile);
  }

  const response = await fetch(API_URL, {
    method: "POST",
    body: formData,
  });

  if (!response.ok) {
    let error;
    try {
      error = await response.json();
    } catch {
      error = { message: "Не удалось добавить анекдот" };
    }
    throw new Error(error.message || "Не удалось добавить анекдот");
  }

  const result = await response.json();
  return result.data;
}

async function deleteJoke(id) {
  const response = await fetch(`${API_URL}/${id}`, {
    method: "DELETE",
  });

  if (!response.ok) {
    let error;
    try {
      error = await response.json();
    } catch {
      error = { message: "Не удалось удалить анекдот" };
    }
    throw new Error(error.message || "Не удалось удалить анекдот");
  }
}

async function togglePin(id) {
  const response = await fetch(`${API_URL}/${id}/pin`, {
    method: "PATCH",
  });

  if (!response.ok) {
    let error;
    try {
      error = await response.json();
    } catch {
      error = { message: "Не удалось переключить закрепление" };
    }
    throw new Error(error.message || "Не удалось переключить закрепление");
  }

  const result = await response.json();
  return result.data;
}

async function toggleArchive(id) {
  const response = await fetch(`${API_URL}/${id}/archive`, {
    method: "PATCH",
  });

  if (!response.ok) {
    let error;
    try {
      error = await response.json();
    } catch {
      error = { message: "Не удалось переключить архив" };
    }
    throw new Error(error.message || "Не удалось переключить архив");
  }

  const result = await response.json();
  return result.data;
}
