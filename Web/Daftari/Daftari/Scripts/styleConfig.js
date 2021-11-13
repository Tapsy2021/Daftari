var retrievedObject = localStorage.getItem('data-layout-config');

if (typeof retrievedObject === 'undefined' || retrievedObject === null) {
	var bodyConfig = '{"leftSideBarTheme":"dark","layoutBoxed":false, "leftSidebarCondensed":false, "leftSidebarScrollable":false,"darkMode":false, "showRightSidebarOnStart": false}';

	localStorage.setItem('data-layout-config', bodyConfig);

	$('body').attr('data-layout-config', bodyConfig)
} else {

	$('body').attr('data-layout-config', retrievedObject);

}

$("#saveConfigBtn").click(function () {
	var config = $.LayoutThemeApp.getConfig();
	var bodyConfig = {
		leftSideBarTheme: config['sideBarTheme'],
		layoutBoxed: config['isBoxed'],
		leftSidebarCondensed: config['isCondensed'],
		leftSidebarScrollable: config['isScrollable'],
		darkMode: config['isDarkModeEnabled']
	};
	localStorage.setItem('data-layout-config', JSON.stringify(bodyConfig));
	console.log("Saved");
});