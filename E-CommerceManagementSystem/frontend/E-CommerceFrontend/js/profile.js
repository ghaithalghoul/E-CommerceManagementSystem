const username = document.getElementById("username");
const email = document.getElementById("email");
const role = document.getElementById("role");

async function getProfile() {

    const token = localStorage.getItem("accessToken");

    if (!token) {
        window.location.href = "login.html";
        return;
    }

    const response = await fetch(
        "https://localhost:7260/api/Auth/me",
        {
            headers: {
                Authorization: `Bearer ${token}`
            }
        }
    );

    if (!response.ok) {
        throw new Error("Failed to load profile");
    }

    const data = await response.json();

    console.log(data);

    username.textContent = data.userName;
    email.textContent = data.email;
    role.textContent = data.role;
}

document.getElementById("logout").addEventListener("click", () => {

    localStorage.removeItem("accessToken");
    localStorage.removeItem("refreshToken");

    window.location.href = "login.html";
});

getProfile();