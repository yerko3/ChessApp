// Llama a la función selectProfile() al cargar la página
document.addEventListener('DOMContentLoaded', function() {
    selectProfile();
});


const API_URL = location.hostname === "localhost" 
    ? "https://localhost:5051"
    : "https://chessapp3-a4chcke6bvhkgwbw.eastus-01.azurewebsites.net";

// Definir las rutas de las imágenes para cada perfi
const profileImages = {
    default: 'images/perfilpre.jpg',
    profile1: 'images/perfil1.gif',
    profile2: 'images/perfil2.gif',
    profile3: 'images/perfil3.gif',
    profile4: 'images/perfil4.gif',
    profile5: 'images/perfil5.gif',
    profile6: 'images/perfil6.gif',
    // Agrega más perfiles según sea necesario
};

let SelectImageUrl = ``;
function selectProfile() {
    const dropdown = document.getElementById('profile-dropdown');
    const preview = document.getElementById('avatar-preview');
    if (!dropdown || !preview)
        return;
    const selectedProfile = dropdown.value;
    SelectImageUrl = profileImages[selectedProfile];
    // Actualizar la imagen de vista previa según la selección
    preview.style.backgroundImage = `url('${profileImages[selectedProfile]}')`;
}


const formRegistro = document.getElementById("miformulario");
if (formRegistro) {
    formRegistro.addEventListener("submit", async function (e) {
        e.preventDefault(); // evita el envío clásico del formulario

        const name = document.getElementById("name").value.trim();
        const email = document.getElementById("email").value.trim();
        const password = document.getElementById("password").value;

        const userData = {
            Name: name,
            Email: email,
            Password: password,
            Image: SelectImageUrl // puedes agregar campo de imagen si tienes
        };

        try {
            const response = await fetch(`${API_URL}/user/create`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(userData)
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            if (response.ok) {
                const result = await response.json();
                alert("Registro exitoso!");
                window.location.href = "perfil.html"; // Redirigir
            } 
        } catch (error) {
            console.error("Error al guardar usuario:", error);
            alert("Error al guardar usuario." + error.message);
        }
    });
}

const loginForm = document.getElementById("loginForm");
if (loginForm) {
    loginForm.addEventListener("submit", async function (e) {
        e.preventDefault();

        const email = document.getElementById("email").value.trim();
        const password = document.getElementById("password").value;


        const loginData = {
            Email: email,
            Password: password
        };

        try {
            const response = await fetch(`${API_URL}/user/login`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                credentials:"include",
                body: JSON.stringify(loginData)
            });
            if (response.ok) {
                const data = await response.json();
                if (data.message === "Login exitoso"){
                    alert("Inicio de sesión exitoso :)");
                    await loadUserProfile();
                    //window.location.href = "usuario.html"; // Redirigir

                }
                
                // Obtener y mostrar datos del usuario usando el JWT de la cookie
                 window.location.href = "usuario.html"; // opcional
            } else if (response.status === 401) {   
                alert("Credenciales inválidas");
            } else {
                const errorText = await response.text();
                alert("Error: " + errorText);
            }
        } catch (error) {
            console.error("Error al iniciar sesión:", error);
            alert("Error al iniciar sesión.");
            loginForm.reset();
        }
    });
}
//document.addEventListener("DOMContentLoaded", () => {
//    const avatar = document.getElementById("user-avatar");
//    const imageUrl = sessionStorage.getItem("userImage");
//    if (avatar && imageUrl) {
//        avatar.src = imageUrl;
//        avatar.style.display = "block"; // 👈 ahora sí se muestra
//    }
//});
// Función para cargar el perfil del usuario usando el JWT de la cookie
async function loadUserProfile() {
    try {
        const response = await fetch(`${API_URL}/user/profile`, {
            method: "GET",
            credentials: "include" // Importante: envía la cookie JWT automáticamente
        });
        
        if (response.ok) {
            const userData = await response.json();
            
            // Mostrar avatar si existe el elemento
            const avatar = document.getElementById("user-avatar");
            if (avatar) {
                avatar.src = userData.image;
                avatar.style.display = "block";
            }
            
            // Mostrar nombre si existe el elemento
            const nameElement = document.getElementById("user-name");
            if (nameElement) {
                nameElement.textContent = userData.name;
                nameElement.style.display = "block";
            }


        } else if (response.status === 401) {
            // Usuario no autenticado, no mostrar nada
            console.log("Usuario no autenticado");
        } else {
            console.error("Error al obtener perfil del usuario");
        }
    } catch (error) {
        console.error("Error al cargar perfil:", error);
    }
}
//hola
// Cargar perfil al cargar la página (si el usuario está autenticado)
document.addEventListener("DOMContentLoaded", () => {
    loadUserProfile();
});

