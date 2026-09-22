const productdetail = document.querySelector(".product-card");

async function getProduct(productId) {

    const response = await fetch(
        `https://localhost:7260/api/products/${productId}`
    );

    if (!response.ok) {
        throw new Error("Failed to load product");
    }

    const data = await response.json();

    const img = document.createElement("img");
    img.src = data.imageUrl;
    img.alt = data.name;

    const productname = document.createElement("h1");
    productname.textContent = data.name;

    const productdescription = document.createElement("p");
    productdescription.textContent = data.description;

    const productprice = document.createElement("p");
    productprice.textContent = `$${data.price}`;

    const productstock = document.createElement("p");
    productstock.textContent = `Stock: ${data.stock}`;

    productdetail.appendChild(img);
    productdetail.appendChild(productname);
    productdetail.appendChild(productdescription);
    productdetail.appendChild(productprice);
    productdetail.appendChild(productstock);
}

const params = new URLSearchParams(window.location.search);

const productId = params.get("id");

if (productId) {
    getProduct(productId);
}