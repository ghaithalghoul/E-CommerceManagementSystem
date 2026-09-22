const token = localStorage.getItem("accessToken");

if (!token) {
    window.location.href = "../login.html";
}

const productsTable =
    document.getElementById("products-table");

const addProductButton =
    document.getElementById("add-product");

const productFormContainer =
    document.getElementById("product-form-container");

const productForm =
    document.getElementById("product-form");

const formTitle =
    document.getElementById("form-title");

const cancelForm =
    document.getElementById("cancel-form");

const productId =
    document.getElementById("product-id");

const productName =
    document.getElementById("product-name");

const productDescription =
    document.getElementById("product-description");

const productPrice =
    document.getElementById("product-price");

const productStock =
    document.getElementById("product-stock");

const productImage =
    document.getElementById("product-image");

const productCategory =
    document.getElementById("product-category");

const previousPage =
    document.getElementById("previous-page");

const nextPage =
    document.getElementById("next-page");

const pageNumber =
    document.getElementById("page-number");

let currentPage = 1;

const pageSize = 10;

let editingProduct = false;

async function getProducts() {

    try {

        const response = await fetch(
            `https://localhost:7260/api/Products?page=${currentPage}&pageSize=${pageSize}`,
            {
                headers: {
                    Authorization: `Bearer ${token}`
                }
            }
        );

        if (response.status === 401 ||
            response.status === 403) {

            window.location.href = "../login.html";
            return;
        }

        if (!response.ok) {
            throw new Error("Failed to load products");
        }

        const data = await response.json();

        console.log("Products:", data);

        productsTable.innerHTML = "";

        if (!data.data || data.data.length === 0) {

            productsTable.innerHTML = `
                <tr>
                    <td colspan="8">
                        No products found
                    </td>
                </tr>
            `;

            return;
        }

        data.data.forEach(product => {

            const row =
                document.createElement("tr");

            row.innerHTML = `
                <td>${product.productId}</td>

                <td>
                    <img
                        src="${
                            product.imageUrl ||
                            "../../assets/images/default-product.jpg"
                        }"
                        alt="${product.name}"
                    >
                </td>

                <td>${product.name}</td>

                <td>${product.description}</td>

                <td>$${product.price}</td>

                <td>${product.stock}</td>

                <td>${product.categoryId}</td>

                <td>

                    <button
                        class="edit-btn"
                        data-id="${product.productId}">
                        Edit
                    </button>

                    <button
                        class="delete-btn"
                        data-id="${product.productId}">
                        Delete
                    </button>

                </td>
            `;

            productsTable.appendChild(row);
        });

        pageNumber.textContent =
            currentPage;

        previousPage.disabled =
            currentPage <= 1;

        nextPage.disabled =
            currentPage >= data.totalPages;

        document
            .querySelectorAll(".edit-btn")
            .forEach(button => {

                button.addEventListener(
                    "click",
                    () => {
                        editProduct(
                            button.dataset.id
                        );
                    }
                );

            });

        document
            .querySelectorAll(".delete-btn")
            .forEach(button => {

                button.addEventListener(
                    "click",
                    () => {
                        deleteProduct(
                            button.dataset.id
                        );
                    }
                );

            });

    } catch (error) {

        console.error(error);

        productsTable.innerHTML = `
            <tr>
                <td colspan="8">
                    Failed to load products
                </td>
            </tr>
        `;
    }
}

function openAddForm() {

    editingProduct = false;

    formTitle.textContent =
        "Add Product";

    productForm.reset();

    productId.value = "";

    productFormContainer
        .classList
        .remove("hidden");
}

function closeForm() {

    productFormContainer
        .classList
        .add("hidden");

    productForm.reset();

    productId.value = "";

    editingProduct = false;
}

async function editProduct(id) {

    try {

        const response = await fetch(
            `https://localhost:7260/api/Products/${id}`
        );

        if (!response.ok) {
            throw new Error(
                "Failed to load product"
            );
        }

        const product =
            await response.json();

        editingProduct = true;

        formTitle.textContent =
            "Edit Product";

        productId.value =
            product.productId;

        productName.value =
            product.name;

        productDescription.value =
            product.description;

        productPrice.value =
            product.price;

        productStock.value =
            product.stock;

        productImage.value =
            product.imageUrl ?? "";

        productCategory.value =
            product.categoryId;

        productFormContainer
            .classList
            .remove("hidden");

        window.scrollTo({
            top: 0,
            behavior: "smooth"
        });

    } catch (error) {

        console.error(error);

        alert("Failed to load product");

    }
}

async function saveProduct(event) {

    event.preventDefault();

    const product = {

        name:
            productName.value,

        description:
            productDescription.value,

        price:
            Number(productPrice.value),

        stock:
            Number(productStock.value),

        imageUrl:
            productImage.value || null,

        categoryId:
            Number(productCategory.value)
    };

    try {

        let response;

        if (editingProduct) {

            response = await fetch(
                `https://localhost:7260/api/Products/update-product/${productId.value}`,
                {
                    method: "PUT",

                    headers: {
                        "Content-Type":
                            "application/json",

                        Authorization:
                            `Bearer ${token}`
                    },

                    body:
                        JSON.stringify(product)
                }
            );

        } else {

            response = await fetch(
                "https://localhost:7260/api/Products/add-product",
                {
                    method: "POST",

                    headers: {
                        "Content-Type":
                            "application/json",

                        Authorization:
                            `Bearer ${token}`
                    },

                    body:
                        JSON.stringify(product)
                }
            );
        }

        if (!response.ok) {

            const error =
                await response.text();

            console.error(error);

            alert(
                editingProduct
                    ? "Failed to update product"
                    : "Failed to add product"
            );

            return;
        }

        closeForm();

        getProducts();

    } catch (error) {

        console.error(error);

        alert("Something went wrong");

    }
}

async function deleteProduct(id) {

    const confirmed =
        confirm(
            "Are you sure you want to delete this product?"
        );

    if (!confirmed) {
        return;
    }

    try {

        const response = await fetch(
            `https://localhost:7260/api/Products/${id}`,
            {
                method: "DELETE",

                headers: {
                    Authorization:
                        `Bearer ${token}`
                }
            }
        );

        if (!response.ok) {

            alert(
                "Failed to delete product"
            );

            return;
        }

        getProducts();

    } catch (error) {

        console.error(error);

        alert("Something went wrong");

    }
}

addProductButton.addEventListener(
    "click",
    openAddForm
);

cancelForm.addEventListener(
    "click",
    closeForm
);

productForm.addEventListener(
    "submit",
    saveProduct
);

previousPage.addEventListener(
    "click",
    () => {

        if (currentPage > 1) {

            currentPage--;

            getProducts();
        }

    }
);

nextPage.addEventListener(
    "click",
    () => {

        currentPage++;

        getProducts();
    }
);

getProducts();