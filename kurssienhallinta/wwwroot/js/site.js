// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", () => {
  console.log("Global scripts loaded");

  const deleteButtons = document.querySelectorAll(".delete-data-btn");

  deleteButtons.forEach((btn) => {
    btn.addEventListener("click", (event) => {
      if (!confirm("Haluatko varmasti poistaa?")) {
        event.preventDefault(); // stop navigation if user cancels
      }
    });
  });
});
