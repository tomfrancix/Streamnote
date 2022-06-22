
const toggleBtn = document.getElementById("toggleBtn");
const toggleTouchBtn = document.getElementById("toggleTouchBtn");
const theme = document.getElementById("mainBody");
let darkMode = localStorage.getItem("dark-mode");

const enableDarkMode = () => {
    theme.classList.add("dark-mode-theme");
    toggleBtn.classList.remove("dark-mode-toggle");
    toggleTouchBtn.classList.remove("dark-mode-toggle");
    localStorage.setItem("dark-mode", "enabled");
};

const disableDarkMode = () => {
    theme.classList.remove("dark-mode-theme");
    toggleBtn.classList.add("dark-mode-toggle");
    toggleTouchBtn.classList.add("dark-mode-toggle");
    localStorage.setItem("dark-mode", "disabled");
};                                             

if (darkMode === "enabled") {
    enableDarkMode(); // set state of darkMode on page load
}

toggleBtn.addEventListener("click", (e) => {
    darkMode = localStorage.getItem("dark-mode"); // update darkMode when clicked
    if (darkMode === "disabled") {
        enableDarkMode();
    } else {
        disableDarkMode();
    }
});

toggleTouchBtn.addEventListener("touchstart", (e) => {
    darkMode = localStorage.getItem("dark-mode"); // update darkMode when clicked
    if (darkMode === "disabled") {
        enableDarkMode();
    } else {
        disableDarkMode();
    }
}); 