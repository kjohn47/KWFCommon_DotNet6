function ExpandEndpointGroup(group_div, endpoint_div_id) {
    let toogled = group_div.getAttribute("toogled");
    let groupDiv = document.getElementById(endpoint_div_id);
    if (toogled === "false") {
        group_div.setAttribute("toogled", "true");
        groupDiv.style.setProperty("display", "block");
        groupDiv.style.setProperty("visibility", "visible");
    }
    else {
        group_div.setAttribute("toogled", "false");
        groupDiv.style.setProperty("display", "none");
        groupDiv.style.setProperty("visibility", "hidden");
    }
}