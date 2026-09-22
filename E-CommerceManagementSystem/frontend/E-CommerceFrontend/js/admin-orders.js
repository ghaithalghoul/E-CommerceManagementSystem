const token = localStorage.getItem("accessToken");

if (!token) {
    window.location.href = "../login.html";
}

const ordersTable =
    document.getElementById("orders-table");

async function getOrders() {

    const response = await fetch(
        "https://localhost:7260/api/Admin/orders?page=1&pageSize=10",
        {
            headers: {
                Authorization: `Bearer ${token}`
            }
        }
    );

    if (!response.ok) {
        throw new Error("Failed to load orders");
    }

    const data = await response.json();

    console.log(data);

    ordersTable.innerHTML = "";

    data.data.forEach(order => {

        const row = document.createElement("tr");

        const date =
            new Date(order.createdAt)
                .toLocaleDateString();

        row.innerHTML = `
            <td>${order.id}</td>
            <td>${order.userId}</td>
            <td>$${order.totalPrice}</td>
            <td>${order.orderStatus}</td>
            <td>${date}</td>
        `;

        ordersTable.appendChild(row);
    });
}

getOrders();