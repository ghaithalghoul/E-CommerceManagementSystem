export async function  getProducts() {
    const response = await fetch("https://localhost:7260/api/products");
    if(!response.ok){
        throw new Error("Failed to fetch products");
    }
    return await response.json();
}
export async function  getCategories() {
    const response = await fetch("https://localhost:7260/api/categories");
    if(!response.ok){
        throw new Error("Failed to fetch Categories");
    }
    return await response.json();
}