// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", () => {
  console.log("Global scripts loaded");

  const select = document.getElementById("modifySelected");
  const form = document.getElementById("courseForm");
  const deleteButtons = document.querySelectorAll(".delete-data-btn");

  select.addEventListener("change", () => {
    if (select.selectedIndex > 0) {
      form.style.display = "block";
    } else {
      form.style.display = "none";
    }
  });

  deleteButtons.forEach((btn) => {
    btn.addEventListener("click", (event) => {
      if (!confirm("Haluatko varmasti poistaa?")) {
        event.preventDefault(); // stop navigation if user cancels
      }
    });
  });
});
