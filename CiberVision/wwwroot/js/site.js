document.addEventListener('DOMContentLoaded', function () {
    // Sidebar toggle for mobile
    const sidebar = document.getElementById('sidebar');
    const toggleBtn = document.getElementById('sidebar-toggle');

    toggleBtn.addEventListener('click', function () {
        sidebar.classList.toggle('active');
    });

    // Smooth scroll for nav links
    document.querySelectorAll('.nav-link').forEach(link => {
        link.addEventListener('click', function (e) {
            e.preventDefault();
            const targetId = this.getAttribute('href');
            if (targetId.startsWith('#')) {
                document.querySelector(targetId).scrollIntoView({ behavior: 'smooth' });
            } else {
                window.location.href = targetId;
            }
            if (window.innerWidth <= 768) {
                sidebar.classList.remove('active');
            }
        });
    });

    // Ensure modal backdrop is removed on nav link click
    document.querySelectorAll('.nav-link').forEach(link => {
        link.addEventListener('click', function () {
            const modal = bootstrap.Modal.getInstance(document.getElementById('resultsModal'));
            if (modal) {
                modal.hide();
            }
        });
    });
});