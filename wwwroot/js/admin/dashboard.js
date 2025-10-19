document.addEventListener('DOMContentLoaded', function () {
 
    // Revenue chart by day
    const revenueCtx = document.getElementById('revenueChart').getContext('2d');
    const revenueChart = new Chart(revenueCtx, {
        type: 'line',
        data: {
            labels: viewData.dailyRevenueChart.Labels, 
            datasets: [{
                label: 'revenue (millions VND)',
                data: viewData.dailyRevenueChart.Data, 
                borderColor: '#667eea',
                backgroundColor: 'rgba(102, 126, 234, 0.1)',
                borderWidth: 3,
                fill: true,
                tension: 0.4,
                pointRadius: 5,
                pointBackgroundColor: '#667eea',
                pointBorderColor: '#fff',
                pointBorderWidth: 2
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { display: false },
                tooltip: {
                    backgroundColor: 'rgba(0, 0, 0, 0.8)',
                    padding: 12,
                    titleColor: '#fff',
                    bodyColor: '#fff',
                    displayColors: false,
                    callbacks: {
                        label: function (context) {
                            return context.parsed.y + 'M VND';
                        }
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    grid: { borderDash: [5, 5] },
                    ticks: {
                        callback: function (value) { return value + 'M'; }
                    }
                },
                x: {
                    grid: { display: false }
                }
            }
        }
    });

    //Revenue Chart by month
    const monthlyCtx = document.getElementById('monthlyChart').getContext('2d');
    const monthlyChart = new Chart(monthlyCtx, {
        type: 'bar',
        data: {
            labels: viewData.monthlyRevenueChart.Labels,
            datasets: [{
                label: 'Incom (millions VND)',
                data: viewData.monthlyRevenueChart.Data,
                backgroundColor: [
                    'rgba(102, 126, 234, 0.8)', 'rgba(118, 75, 162, 0.8)',
                    'rgba(240, 147, 251, 0.8)', 'rgba(245, 87, 108, 0.8)',
                    'rgba(79, 172, 254, 0.8)', 'rgba(0, 242, 254, 0.8)'
                ],
                borderRadius: 8,
                borderWidth: 0
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { display: false },
                tooltip: {
                    backgroundColor: 'rgba(0, 0, 0, 0.8)',
                    padding: 12,
                    callbacks: {
                        label: function (context) {
                            return context.parsed.y + 'M VND';
                        }
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    grid: { borderDash: [5, 5] },
                    ticks: {
                        callback: function (value) { return value + 'M'; }
                    }
                },
                x: {
                    grid: { display: false }
                }
            }
        }
    });

    // Animate occupancy circle
    const circle = document.getElementById('occupancyCircle');
    if (circle) {
        const radius = circle.r.baseVal.value;
        const circumference = radius * 2 * Math.PI;
        const occupancyRate = viewData.occupancyRate;
        const offset = circumference - (occupancyRate / 100) * circumference;

        circle.style.strokeDasharray = `${circumference} ${circumference}`;
        circle.style.strokeDashoffset = circumference;

        setTimeout(() => {
            circle.style.transition = 'stroke-dashoffset 0.8s ease-out';
            circle.style.strokeDashoffset = offset;
        }, 100);
    }
});