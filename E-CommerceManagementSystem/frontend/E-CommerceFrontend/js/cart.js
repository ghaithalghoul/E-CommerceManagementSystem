const cartitems = document.querySelector(".cart-items");
const clearcart = document.querySelector(".clear-cart-btn");
clearcart.addEventListener("click", deleteCart);

const continueshoppingbtn=  document.querySelector(".continue-shopping-btn")
continueshoppingbtn.addEventListener("click", () => {
        window.location.href = "products.html";
    });
async function getCAart(){
    cartitems.innerHTML = "";
    const token = localStorage.getItem("accessToken");
    const response = await fetch("https://localhost:7260/api/cart",
        {
            headers:{
                Authorization : `Bearer ${token}`
            }
        }
    );
    if(!response.ok){
        throw new Error("Failed to load Cart");
    }
    const data = await response.json();
    console.log(data);
    if(data.items.length == 0){
        const emptycart = document.createElement("div");
        emptycart.innerHTML =`
        <p>Your Cart is Empty</p>
        <p>You haven't added any products yet.</p>
        <p>[ <a href="products.html>Continue Shopping</a>]</p>
        `;
        cartitems.appendChild(emptycart);
        document.querySelector("#sub-total").textContent = "$ 0";
        document.querySelector("#total-price").textContent = "$ 0";
        return;
    }else{
        data.items.forEach(item => {

        const cartItem = document.createElement("div");
        
        cartItem.innerHTML = `
        <img src="${item.ImageUrl} >
        <h3>${item.productName}</h3>

        <p>Price: $${item.price}</p>

        <button
            onclick="updateCartitem(${item.id}, ${item.quantity - 1})"
            ${item.quantity <= 1 ? "disabled" : ""}
        >
            -
        </button>

        <p>Quantity: ${item.quantity}</p>

        <button
            onclick="updateCartitem(${item.id}, ${item.quantity + 1})"
        >
            +
        </button>

        <p>Total: $${item.total}</p>

        <button
            onclick="deleteCartItem(${item.id})"
        >
            Remove
        </button>
        `;

        cartitems.appendChild(cartItem);
        });
        const subtotal = document.querySelector("#sub-total");
        const totalprice = document.querySelector("#total-price");
        const total = Number(data.total);
        subtotal.textContent = `$ ${total}`;
        totalprice.textContent = `$ ${total+5}`;
    }
    
}
async function AddToCart(productId, quantity) {
    const token = localStorage.getItem("accessToken");
    const response = await fetch("https://localhost:7260/api/cart",
        {
            method:"POST",
            headers:{
                Authorization: `Bearer ${token}`,
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                ProductId:productId,
                Quantity:quantity
            })
        }
    );
    if (!response.ok) {
        throw new Error("Failed to add product to cart");
    }
    const data = await response.json();

    console.log(data);
}
async function updateCartitem(cartitemId, quantity) {
    const token = localStorage.getItem("accessToken");
    const response = await fetch(`https://localhost:7260/api/cart?cartitemId=${cartitemId}`,
        {
            method:"PUT",
            headers:{
                "Content-Type": "application/json",
                Authorization: `Bearer ${token}`
            },
            body: JSON.stringify({
                Quantity:quantity
            })
            
        }
        
    );
    if (!response.ok) {
        throw new Error("Failed to update cart item");
    }

    const data = await response.json();
    await getCAart();
    console.log(data);
}
async function  deleteCartItem(cartItemId) {
    const token = localStorage.getItem("accessToken");
    const response = await fetch(`https://localhost:7260/api/cart/${cartItemId}`,{
        method:"DELETE",
        headers:{
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`
        }
    });
    if (!response.ok) {
        throw new Error("Failed to delete cart item");
        
    }

    const data = await response.json();
    await getCAart();
}
async function  deleteCart() {
    const token = localStorage.getItem("accessToken");
    const response =await fetch("https://localhost:7260/api/cart/delete-cart",{
        method:"DELETE",
        headers:{
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`
        }
    });
    if (!response.ok) {
        throw new Error("Failed to delete cart ");
    }
    await getCAart();
    const data = await response.json();
}
const checkoutbtn = document.querySelector(".checkout-btn");
checkoutbtn.addEventListener("click",async function (event) {
    const token = localStorage.getItem("accessToken");
    var response = await fetch("https://localhost:7260/api/order",{
        method:"POST",
        headers:{
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`
        }
    });
    if (!response.ok) {
        throw new Error("Failed to Create order ");
    }

    const data = await response.json();
    const orderresponse = document.createElement("div");
    orderresponse.innerHTML = `
    <p>Order placed successfully!</p>
    <p>Order #${data.Id}</p>
    <p>Total: $ ${data.TotalPrice}</p>
    `
    document.body.appendChild(orderresponse);

    setTimeout(() => {
        window.location.href = "orders.html";
    }, 1500);
    console.log(data);
})
window.updateCartitem = updateCartitem;
window.deleteCartItem = deleteCartItem;

getCAart();