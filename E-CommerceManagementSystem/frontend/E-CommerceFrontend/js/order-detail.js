const order_id = document.getElementById("order-id");
const orderdate = document.getElementById("order-date");
const orderstate = document.getElementById("order-stat");
const orderitems = document.querySelector(".order-items");
const ordersummary = document.querySelector(".order-summary");

async function getOrder(orderid) {

    const token = localStorage.getItem("accessToken");

    const response = await fetch(
        `https://localhost:7260/api/order/${orderid}`,
        {
            headers: {
                Authorization: `Bearer ${token}`
            }
        }
    );

    if (!response.ok) {
        throw new Error("Failed to load order");
    }

    const data = await response.json();

    order_id.textContent = `Order #${data.id}`;

    const date = new Date(data.createdAt);
    orderdate.textContent = `Placed on: ${date.toLocaleDateString()}`;

    orderstate.textContent = data.orderStatus;

    orderitems.innerHTML = "";

    data.items.forEach(element => {

        const ordercard = document.createElement("div");
        ordercard.classList.add("order-item");

        const orderitemimg = document.createElement("img");

        if (element.imageUrl) {
            orderitemimg.src = element.imageUrl;
        } else {
            orderitemimg.src = "../assets/images/default-product.jpg";
        }

        orderitemimg.alt = element.productName;

        const orderitemdetail = document.createElement("div");

        orderitemdetail.innerHTML = `
            <h3>${element.productName}</h3>
            <p>Price: $${element.unitPrice}</p>
            <p>Quantity: ${element.quantity}</p>
            <p>Total: $${element.total}</p>
        `;

        ordercard.appendChild(orderitemimg);
        ordercard.appendChild(orderitemdetail);

        orderitems.appendChild(ordercard);
    });

    const totalprice = Number(data.totalPrice);

    ordersummary.innerHTML = `
        <h2>Order Summary</h2>

        <p>Items: $${totalprice}</p>

        <p>Shipping: $5</p>

        <hr>

        <h3>Total: $${totalprice + 5}</h3>

        ${
            data.orderStatus === "Pending" ||
            data.orderStatus === "Processing"
                ? `<button class="cancel-order-btn" onclick="cancelOrder(${data.id})">
                    Cancel Order
                   </button>`
                : ""
        }
    `;
}

async function cancelOrder(orderid) {

    const token = localStorage.getItem("accessToken");

    const response = await fetch(
        `https://localhost:7260/api/order/cancel/${orderid}`,
        {
            method: "PUT",
            headers: {
                Authorization: `Bearer ${token}`
            }
        }
    );

    if (!response.ok) {
        throw new Error("Failed to cancel order");
    }

    const ordercanceled = document.querySelector(".order-canceled");

    ordercanceled.textContent = "Your order has been canceled";

    setTimeout(() => {
        window.location.href = "orders.html";
    }, 1500);
}

const params = new URLSearchParams(window.location.search);
const orderid = params.get("id");

if (orderid) {
    getOrder(orderid);
}