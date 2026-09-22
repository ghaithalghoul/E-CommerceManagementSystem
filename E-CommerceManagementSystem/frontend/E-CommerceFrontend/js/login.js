const loginForm = document.getElementById("loginForm");

function getUserRole(token) {

    const payload = JSON.parse(
        atob(token.split(".")[1])
    );

    return payload[
        "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
    ];
}

loginForm.addEventListener("submit", async function (event) {

    event.preventDefault();

    const usernameOrEmail =
        document.getElementById("usernameOrEmail").value;

    const password =
        document.getElementById("password").value;

    const response = await fetch(
        "https://localhost:7260/api/auth/login",
        {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                usernameOrEmail: usernameOrEmail,
                password: password
            })
        }
    );

    const data = await response.json();

    if (response.ok) {

        localStorage.setItem(
            "accessToken",
            data.accessToken
        );

        localStorage.setItem(
            "refreshToken",
            data.refreshToken
        );

        const role = getUserRole(data.accessToken);

        if (role === "Admin") {

            window.location.href =
                "admin/dashboard.html";

        } else {

            window.location.href =
                "../index.html";

        }

    } else {

        console.log(data);

        alert("Invalid username or password");
    }
});