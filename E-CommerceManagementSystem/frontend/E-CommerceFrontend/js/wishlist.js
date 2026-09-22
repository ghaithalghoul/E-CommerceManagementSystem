const wishlistcontainer =
    document.querySelector(".wishlist-container");

async function getWishlist() {

    const token = localStorage.getItem("accessToken");

    if (!token) {
        window.location.href = "login.html";
        return;
    }

    const response = await fetch(
        "https://localhost:7260/api/Wishlist",
        {
            headers: {
                Authorization: `Bearer ${token}`
            }
        }
    );

    if (!response.ok) {
        throw new Error("Failed to load wishlist");
    }

    const data = await response.json();

    console.log(data);

    wishlistcontainer.innerHTML = "";

    if (data.length === 0) {

        wishlistcontainer.innerHTML = `
            <div class="empty-wishlist">
                <h2>Your Wishlist is Empty</h2>
                <p>You haven't added any products yet.</p>
                <a href="products.html">Browse Products</a>
            </div>
        `;

        return;
    }

    data.forEach(item => {

        const card = document.createElement("div");
        card.classList.add("wishlist-card");

        const image = document.createElement("img");

        image.src =
            item.imageUrl ||
            "../assets/images/default-product.jpg";

        image.alt = item.productName;

        const info = document.createElement("div");
        info.classList.add("wishlist-info");

        const name = document.createElement("h2");
        name.textContent = item.productName;

        const price = document.createElement("p");
        price.textContent = `$${item.price}`;

        const buttons = document.createElement("div");
        buttons.classList.add("wishlist-buttons");

        const cartButton = document.createElement("button");
        cartButton.textContent = "Add to Cart";

        cartButton.addEventListener("click", () => {
            addToCart(item.productId);
        });

        const removeButton = document.createElement("button");
        removeButton.textContent = "Remove";

        removeButton.addEventListener("click", () => {
            removeFromWishlist(item.productId);
        });

        buttons.appendChild(cartButton);
        buttons.appendChild(removeButton);

        info.appendChild(name);
        info.appendChild(price);
        info.appendChild(buttons);

        card.appendChild(image);
        card.appendChild(info);

        wishlistcontainer.appendChild(card);
    });
}

async function removeFromWishlist(productId) {

    const token = localStorage.getItem("accessToken");

    const response = await fetch(
        `https://localhost:7260/api/Wishlist/${productId}`,
        {
            method: "DELETE",
            headers: {
                Authorization: `Bearer ${token}`
            }
        }
    );

    if (!response.ok) {
        throw new Error("Failed to remove product");
    }

    getWishlist();
}

async function addToCart(productId) {

    const token = localStorage.getItem("accessToken");

    const response = await fetch(
        "https://localhost:7260/api/Cart",
        {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${token}`
            },
            body: JSON.stringify({
                productId: productId,
                quantity: 1
            })
        }
    );

    if (!response.ok) {
        throw new Error("Failed to add product to cart");
    }

    alert("Product added to cart");
}

getWishlist();