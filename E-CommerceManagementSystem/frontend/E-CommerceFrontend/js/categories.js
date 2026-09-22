import { getCategories } from "./api.js";
const categoriescontainer = document.querySelector("#categories-container");
async function LoadCategories() {
    var categories = await getCategories();
    console.log(categories);
    categories.forEach(element => {
       const categorycard = document.createElement("div");
       categorycard.classList.add("category-card");
       const categoryimage = document.createElement("div");
       categoryimage.classList.add("category-image");
       const img = document.createElement("img");
       const categoryname = document.createElement("h3");
       const aelement = document.createElement("a");
       img.src = `assets/images/${element.name}.jpg`;
       img.alt = `${element.name}`;
       categoryname.textContent = `${element.name}`;
       aelement.textContent = "View Products";
       categoryimage.appendChild(img);
       categorycard.appendChild(categoryimage);
       categorycard.appendChild(categoryname);
       categorycard.appendChild(aelement);
       categoriescontainer.appendChild(categorycard);



    });
}
LoadCategories();