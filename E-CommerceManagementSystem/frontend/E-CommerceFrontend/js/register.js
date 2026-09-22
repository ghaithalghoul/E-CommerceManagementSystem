const registerform = document.querySelector("#registerForm");
registerform.addEventListener("submit",async function(event) {
    event.preventDefault();
    const username = document.querySelector("#username");
    const email = document.querySelector("#email");
    const password = document.querySelector("#password");
    const checkpassword = document.querySelector("#checkpassword");
    if(password.value != checkpassword.value){
        throw new Error("the password should match");
    }
    var response = await fetch("https://localhost:7260/api/auth/register",
        {method:"POST",
        headers:{
            "Content-Type": "application/json"
        },
        body:JSON.stringify({
            username:username.value,
            email:email.value,
            password:password.value
        })
    });
    const data = response.json();
    if( response.ok){
        console.log(data);
        window.location.href = "login.html";
    }else{
        console.error(response);
    }
})