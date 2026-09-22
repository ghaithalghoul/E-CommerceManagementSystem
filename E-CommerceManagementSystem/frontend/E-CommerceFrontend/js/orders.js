const ordercontainer = document.querySelector(".orders-container");

async function getOrders() {
    const token = localStorage.getItem("accessToken");

    const response = await fetch(
        "https://localhost:7260/api/order",
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

    ordercontainer.innerHTML = "";

    if (data.length === 0) {
        const emptyMessage = document.createElement("div");

        emptyMessage.innerHTML = `
            <h2>No Orders Yet</h2>
            <p>You haven't placed any orders yet.</p>
            <a href="products.html">Start Shopping</a>
        `;

        ordercontainer.appendChild(emptyMessage);
        return;
    }

    data.forEach(element => {

        const ordercard = document.createElement("div");
        ordercard.classList.add("order-card");

        const orderheader = document.createElement("div");
        orderheader.classList.add("order-header");

        const orderid = document.createElement("h2");
        orderid.textContent = `Order #${element.id}`;

        const orderstatus = document.createElement("span");
        orderstatus.textContent = element.orderStatus;
        orderstatus.classList.add("order-status");

        orderheader.appendChild(orderid);
        orderheader.appendChild(orderstatus);

        const orderdate = document.createElement("p");
        const date = new Date(element.createdAt);

        orderdate.textContent =
            `Placed on: ${date.toLocaleDateString()}`;

        ordercard.appendChild(orderheader);
        ordercard.appendChild(orderdate);

        const itemscontainer = document.createElement("div");
        itemscontainer.classList.add("order-items");

        element.items.forEach(item => {

            const orderitemcard = document.createElement("div");
            orderitemcard.classList.add("order-item");

            const image = document.createElement("img");
            image.src = item.imageUrl || "../assets/images/default-product.jpg";
            image.alt = item.productName;

            const productinfo = document.createElement("div");
            productinfo.classList.add("product-info");

            const productname = document.createElement("h3");
            productname.textContent = item.productName;

            const itemprice = document.createElement("p");
            itemprice.textContent = `Price: $${item.unitPrice}`;

            const itemquantity = document.createElement("p");
            itemquantity.textContent = `Quantity: ${item.quantity}`;

            const itemtotal = document.createElement("p");
            itemtotal.textContent = `Total: $${item.total}`;

            productinfo.appendChild(productname);
            productinfo.appendChild(itemprice);
            productinfo.appendChild(itemquantity);
            productinfo.appendChild(itemtotal);

            orderitemcard.appendChild(image);
            orderitemcard.appendChild(productinfo);

            itemscontainer.appendChild(orderitemcard);
        });

        ordercard.appendChild(itemscontainer);

        const orderfooter = document.createElement("div");
        orderfooter.classList.add("order-footer");

        const total = document.createElement("strong");
        total.textContent = `Total: $${element.totalPrice}`;

        const detailsbutton = document.createElement("button");
        detailsbutton.textContent = "View Details";

        detailsbutton.addEventListener("click", () => {
            window.location.href =
                `order-details.html?id=${element.id}`;
        });

        orderfooter.appendChild(total);
        orderfooter.appendChild(detailsbutton);

        ordercard.appendChild(orderfooter);

        ordercontainer.appendChild(ordercard);
    });
}

getOrders();