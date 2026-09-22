const productscontainer = document.querySelector("#products-container");

import { getProducts } from "./api.js";
import { createProductCard } from "./product-card.js";

async function loadProducts() {

    const products = await getProducts();

    products.data.forEach(product => {

        const card = createProductCard(
            product,
            "assets/images/"
        );

        productscontainer.appendChild(card);
    });
}

loadProducts();