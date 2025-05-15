// pie chart
google.charts.load("current", { packages: ["corechart"] });
google.charts.setOnLoadCallback(drawPieChart);
function drawPieChart(newData = []) {
	if (newData.length === 0) {
		return;
	}

	var data = google.visualization.arrayToDataTable([
		['Quảng cáo', ''],
		...newData
	]);

	var options = {
		is3D: true,
		focusTarget: 'category',
		backgroundColor: 'transparent',
		fontName: 'Open Sans',
		chartArea: {
			left: 50,
			top: 10,
			width: '80%',
			height: '80%'
		},
		bar: {
			groupWidth: '80%'
		},
		hAxis: {
			textStyle: {
				fontSize: 11
			}
		},
		vAxis: {
			minValue: 0,
			maxValue: 1500,
			baselineColor: '#DDD',
			gridlines: {
				color: '#DDD',
				count: 4
			},
			textStyle: {
				fontSize: 11
			}
		},
		legend: {
			position: 'bottom',
			textStyle: {
				fontSize: 12
			}
		},
		animation: {
			duration: 1200,
			easing: 'out',
			startup: true
		}
	};

	var chart = new google.visualization.PieChart(document.getElementById('pie-chart'));
	chart.draw(data, options);
}

// line chart
google.charts.load("current", { packages: ['corechart'] });
google.charts.setOnLoadCallback(drawLineChart);

function randomIntFromInterval(min, max) { // min and max included
	return Math.floor(Math.random() * (max - min + 1) + min)
}


function drawLineChart(newData = []) {
	if (newData.length === 0) {
		return;
	}

	var data = google.visualization.arrayToDataTable([
		['Ngày', 'Doanh thu'],
		...newData,
	]);

	const dateNow = new Date();
	let currentMonth = dateNow.getMonth() + 1
	currentMonth = currentMonth < 10 ? '0' + currentMonth : currentMonth;
	const currentYear = dateNow.getFullYear();

	var lineOptions = {
		backgroundColor: 'transparent',
		colors: ['cornflowerblue', 'tomato'],
		fontName: 'Open Sans',
		// focusTarget: 'category',
		chartArea: {
			left: 50,
			top: 10,
			width: '80%',
			height: '80%'
		},
		hAxis: {
			title: `Tháng ${currentMonth}/${currentYear}`
		},
		vAxis: {
			title: 'Tổng tiền'
		},
		legend: {
			position: 'bottom',
			textStyle: {
				fontSize: 12
			}
		},
		animation: {
			duration: 1200,
			easing: 'out',
			startup: true
		}
	};

	var chart = new google.visualization.LineChart(document.getElementById('line-chart'));

	chart.draw(data, lineOptions);
}

$(document).ready(function () {
	$('#action_menu_btn').click(function () {
		$('.action_menu').toggle();
	});

	const loadHTML = `<p class="text-center text-success" style="font-weight: bold;">
                                <span class="glyphicon glyphicon-refresh spinning"></span> Đang tải dữ liệu...
                              </p>`;

	$('#loadHTML').html(loadHTML);

	// call api top three product statistic
	$.ajax({
		contentType: 'application/json',
		url: '/Admin/Dashboard/TopThreeProductStatistic',
		success: function (data) {
			const totalMoneyOfThreeProduct = data.reduce((prev, item) => prev += item.CountOrder, 0);
			const totalOrder = +($('#totalOrder').text());
			data.push({
				ProductName: 'Khác',
				CountOrder: totalOrder - totalMoneyOfThreeProduct,
			});

			let newData = [];
			for (item of data) {
				newData.push([item.ProductName, item.CountOrder]);
			}

			// console.log('===> TopThreeProductStatistic: ', newData);

			setTimeout(() => {
				$('#loadHTML').remove();
				$('#areaChart').show();
				drawPieChart(newData);
			}, 500);

		},
		error: function (err) {
			console.log('===> err: ', err);
		}
	});

	// call api revenue of current month statistic
	$.ajax({
		contentType: 'application/json',
		url: '/Admin/Dashboard/RevenueOfCurrentMonthStatistic',
		success: function (data) {
			let newData = [];
			for (let item of data) {
				const day = item.Day < 10 ? '0' + item.Day : item.Day;
				newData.push([`${day}`, item.Revenue]);
			}

			// console.log('===> RevenueOfCurrentMonthStatistic: ', newData);

			setTimeout(() => {
				$('#loadHTML').remove();
				$('#areaChart').show();
				drawLineChart(newData);
			}, 500);
		},
		error: function (err) {
			console.log('===> err: ', err);
		}
	});
});