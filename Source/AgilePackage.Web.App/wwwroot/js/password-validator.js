const checkPassword = (value) => {
    const hasLower = value.match(/[a-z]/);
    const hasUpper = value.match(/[A-Z]/);
    const hasNumber = value.match(/\d+/g);
    const hasLength = value.length >= 8;
    return ({ hasLower, hasUpper, hasNumber, hasLength, })
}
const updateInterface = () => {
    const passwordInput = document.getElementById("password")
    const validation = checkPassword(passwordInput.value)
    const lowercase = document.getElementById("password-validator-ui-lowercase")
    const uppercase = document.getElementById("password-validator-ui-uppercase")
    const number = document.getElementById("password-validator-ui-number")
    const characters = document.getElementById("password-validator-ui-characters")
    if (validation.hasLower) {
        lowercase.classList.add("badge-success")
    } else {
        lowercase.classList.remove("badge-success")
    }
    if (validation.hasUpper) {
        uppercase.classList.add("badge-success")
    } else {
        uppercase.classList.remove("badge-success")
    }
    if (validation.hasNumber) {
        number.classList.add("badge-success")
    } else {
        number.classList.remove("badge-success")
    }
    if (validation.hasLength) {
        characters.classList.add("badge-success")
    } else {
        characters.classList.remove("badge-success")
    }
}
const passwordInput = document.getElementById("password")
passwordInput.addEventListener("input", () => updateInterface())