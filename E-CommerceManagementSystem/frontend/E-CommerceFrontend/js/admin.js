const token = localStorage.getItem("accessToken");

if (!token) {
    window.location.href = "../login.html";
}

async function getDashboard() {

    const response = await fetch(
        "https://localhost:7260/api/Admin/dashboard",
        {
            headers: {
                Authorization: `Bearer ${token}`
            }
        }
    );

    if (!response.ok) {
        throw new Error("Failed to load dashboard");
    }

    const data = await response.json();

    console.log(data);

    document.getElementById("total-users").textContent =
        data.totalUsers ?? 0;

    document.getElementById("total-products").textContent =
        data.totalProducts ?? 0;

    document.getElementById("total-orders").textContent =
        data.totalOrders ?? 0;

    document.getElementById("total-reviews").textContent =
        data.totalReviews ?? 0;

    document.getElementById("total-revenue").textContent =
        `$${data.totalRevenue ?? 0}`;
}

getDashboard();