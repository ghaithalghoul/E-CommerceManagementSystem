export function createProductCard(product, imagePath) {

    const productcard = document.createElement("div");
    productcard.classList.add("product-card");

    const productimage = document.createElement("div");
    productimage.classList.add("product-image");

    const productinfo = document.createElement("div");
    productinfo.classList.add("product-info");

    const img = document.createElement("img");

    const productname = document.createElement("h3");

    const productprice = document.createElement("p");
    productprice.classList.add("price");

    const productdetail = document.createElement("a");
    productdetail.classList.add("details-btn");
    productdetail.textContent = "View Details";
    productdetail.href = `pages/product-detail.html?id=${product.productId}`;

    const producttocart = document.createElement("button");
    producttocart.classList.add("cart-btn");
    producttocart.textContent = "Add to Cart";

    img.src = `${imagePath}${product.name}.jpg`;
    img.alt = product.name;

    productname.textContent = product.name;
    productprice.textContent = `$${product.price}`;

    producttocart.addEventListener("click", async () => {

        const token = localStorage.getItem("accessToken");

        if (!token) {
            window.location.href = "login.html";
            return;
        }

        try {

            const response = await fetch(
                "https://localhost:7260/api/cart",
                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        "Authorization": `Bearer ${token}`
                    },
                    body: JSON.stringify({
                        productId: product.productId,
                        quantity: 1
                    })
                }
            );

            if (!response.ok) {
                throw new Error("Failed to add product to cart");
            }

            producttocart.textContent = "Added ✓";

            setTimeout(() => {
                producttocart.textContent = "Add to Cart";
            }, 1500);

        } catch (error) {
            console.error(error);
            alert("Failed to add product to cart");
        }
    });

    productimage.appendChild(img);

    productinfo.appendChild(productname);
    productinfo.appendChild(productprice);
    productinfo.appendChild(productdetail);
    productinfo.appendChild(producttocart);

    productcard.appendChild(productimage);
    productcard.appendChild(productinfo);

    return productcard;
}