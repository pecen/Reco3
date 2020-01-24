// *** Top Menu Functions
function configureTopMenu() {
    $("#mainMenuLevelOne>li.active").removeClass("active");
    $("#menuLevelTwo>ul").hide();

    selectCurrentFirstLevelMenu();
    showCurrentSecondLevelMenu();
    selectCurrentSecondLevelMenu();
}

// First Level Functions - Initial
function clickFirstLevelMenu(sender, urlTarget)
{
    var firstLevelId = $(sender).parent().attr("id");
    sessionStorage.setItem("TopMenuFirstLevel", firstLevelId);

    if (urlTarget != '') {
        window.location = urlTarget;
        sessionStorage.setItem("TopMenuSecondLevel", undefined);
        sessionStorage.setItem("TopMenuSecondLevelItem", undefined);
    }
    else {
        sessionStorage.setItem("TopMenuSecondLevel", firstLevelId + "_LevelTwo");
        configureTopMenu();
    }
}

function selectCurrentFirstLevelMenu() {

    if (sessionStorage.getItem("TopMenuFirstLevel") != undefined) {

        var itemName = sessionStorage.getItem("TopMenuFirstLevel");

        $("#" + itemName).addClass("active");

        //Check if exists the Second Level, and if true, show it
        var secondLevel = $("#menuLevelTwo>ul[id=" + itemName + "_LevelTwo]");

        if (secondLevel != undefined) {
            sessionStorage.setItem("TopMenuSecondLevel", $(secondLevel).attr("id"));
        }
        else {
            sessionStorage.setItem("TopMenuSecondLevel", undefined);
        }
    }
}
// First Level Functions - Finish

// Second Level Functions - Initial
function clickSecondLevelMenu(sender, urlTarget) {
    var secondLevelId = $(sender).parent().attr("id");
    sessionStorage.setItem("TopMenuSecondLevelItem", secondLevelId);

    if (urlTarget != '') {
        window.location = urlTarget;
    }
}

function showCurrentSecondLevelMenu() {

    if (sessionStorage.getItem("TopMenuSecondLevel") != undefined) {

        var itemName = sessionStorage.getItem("TopMenuSecondLevel");

        $("#" + itemName).show();
    }
}

function selectCurrentSecondLevelMenu() {

    if (sessionStorage.getItem("TopMenuSecondLevelItem") != undefined) {

        var itemName = sessionStorage.getItem("TopMenuSecondLevelItem");

        $("#" + itemName).addClass("active");
    }
}
// Second Level Functions - Finish


// Lefet Menu - Initial
function selectMnuLeftDevItem(mnuName, mnuItemName) {
    $("#" + mnuName + ">ul>li.active").removeClass("active");
    $("#" + mnuItemName).addClass("active");
}
// Lefet Menu - Finish