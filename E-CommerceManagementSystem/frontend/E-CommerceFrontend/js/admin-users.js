const token = localStorage.getItem("accessToken");

if (!token) {
    window.location.href = "../login.html";
}

const usersTable = document.getElementById("users-table");

const previousPage = document.getElementById("previous-page");
const nextPage = document.getElementById("next-page");
const pageNumber = document.getElementById("page-number");

let currentPage = 1;

const pageSize = 10;

async function getUsers() {

    const response = await fetch(
        `https://localhost:7260/api/Admin/users?page=${currentPage}&pageSize=${pageSize}`,
        {
            headers: {
                Authorization: `Bearer ${token}`
            }
        }
    );

    if (!response.ok) {
        throw new Error("Failed to load users");
    }

    const data = await response.json();

    console.log(data);

    usersTable.innerHTML = "";

    data.data.forEach(user => {

        const row = document.createElement("tr");

        row.innerHTML = `
            <td>${user.id}</td>
            <td>${user.userName}</td>
            <td>${user.email}</td>
            <td>${user.role}</td>
            <td>
                <button class="edit-btn"
                    data-id="${user.id}">
                    Change Role
                </button>

                <button class="delete-btn"
                    data-id="${user.id}">
                    Delete
                </button>
            </td>
        `;

        usersTable.appendChild(row);
    });

    pageNumber.textContent = currentPage;

    previousPage.disabled = currentPage <= 1;

    nextPage.disabled =
        currentPage >= data.totalPages;

    document.querySelectorAll(".delete-btn")
        .forEach(button => {

            button.addEventListener("click", () => {
                deleteUser(button.dataset.id);
            });

        });

    document.querySelectorAll(".edit-btn")
        .forEach(button => {

            button.addEventListener("click", () => {
                changeRole(button.dataset.id);
            });

        });
}

async function deleteUser(id) {

    const confirmed =
        confirm("Are you sure you want to delete this user?");

    if (!confirmed) return;

    const response = await fetch(
        `https://localhost:7260/api/Admin/delete-user/${id}`,
        {
            method: "DELETE",
            headers: {
                Authorization: `Bearer ${token}`
            }
        }
    );

    if (!response.ok) {
        alert("Failed to delete user");
        return;
    }

    getUsers();
}

async function changeRole(id) {

    const role = prompt(
        "Enter new role: Admin or Customer"
    );

    if (!role) return;

    const response = await fetch(
        `https://localhost:7260/api/Admin/update-user-role/${id}`,
        {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${token}`
            },
            body: JSON.stringify({
                role: role
            })
        }
    );

    if (!response.ok) {
        alert("Failed to change role");
        return;
    }

    getUsers();
}

previousPage.addEventListener("click", () => {

    if (currentPage > 1) {
        currentPage--;
        getUsers();
    }

});

nextPage.addEventListener("click", () => {

    currentPage++;
    getUsers();

});

getUsers();