const hideAllQuebabs = () => {
    const elements = document.querySelectorAll(".quebab")
    elements.forEach((element) => {
        element.style.display = "none"
    })
}

const openQuebabMenu = (id) => {
    hideAllQuebabs()

    const element = document.querySelector(`#quebab-menu-${id}`)
    if (!element) {
        return
    }

    element.style.display = "inline"

    window.addEventListener('mouseup', (event) => {
        if (event.target != element && event.target.parentNode != element) {
            element.style.display = 'none';
        }
    });
}