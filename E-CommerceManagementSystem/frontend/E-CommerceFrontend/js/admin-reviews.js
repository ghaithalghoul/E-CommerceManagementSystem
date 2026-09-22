const token = localStorage.getItem("accessToken");

if (!token) {
    window.location.href = "../login.html";
}

const reviewsTable =
    document.getElementById("reviews-table");

async function getReviews() {

    const response = await fetch(
        "https://localhost:7260/api/Admin/reviews?page=1&pageSize=10",
        {
            headers: {
                Authorization: `Bearer ${token}`
            }
        }
    );

    if (!response.ok) {
        throw new Error("Failed to load reviews");
    }

    const data = await response.json();

    console.log(data);

    reviewsTable.innerHTML = "";

    data.data.forEach(review => {

        const row = document.createElement("tr");

        row.innerHTML = `
            <td>${review.id}</td>
            <td>${review.userName}</td>
            <td>${review.productName}</td>
            <td>${review.rating}/5</td>
            <td>${review.comment}</td>

            <td>
                <button
                    class="delete-btn"
                    data-id="${review.id}">
                    Delete
                </button>
            </td>
        `;

        reviewsTable.appendChild(row);
    });

    document.querySelectorAll(".delete-btn")
        .forEach(button => {

            button.addEventListener("click", () => {
                deleteReview(button.dataset.id);
            });

        });
}

async function deleteReview(id) {

    const confirmed =
        confirm("Are you sure you want to delete this review?");

    if (!confirmed) return;

    const response = await fetch(
        `https://localhost:7260/api/Admin/delete-review/${id}`,
        {
            method: "DELETE",
            headers: {
                Authorization: `Bearer ${token}`
            }
        }
    );

    if (!response.ok) {
        alert("Failed to delete review");
        return;
    }

    getReviews();
}

getReviews();