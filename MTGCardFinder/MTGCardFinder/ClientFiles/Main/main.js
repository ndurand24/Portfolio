let url = "https://localhost:7101"

$(() => {
    $("[name=logout]").on("click", function(){
        MakeAjaxCall(url + "/logout", "POST", {}, "json", LogoutSuccess, AjaxError);
    })
})

function LogoutSuccess(jsonData)
{
    console.log(jsonData);
    window.location.replace("../Login/login.html");
}

function MakeAjaxCall(serverURL, reqMethod, data, serverResp, successCallback, errorCallback)
{
    let ajaxData = {};
    ajaxData.url = serverURL;
    ajaxData.type = reqMethod;
    ajaxData.data = JSON.stringify(data);
    ajaxData.dataType = serverResp;
    ajaxData.success = successCallback;
    ajaxData.error = errorCallback;
    ajaxData.contentType = "application/json";
    ajaxData.xhrFields = {
        withCredentials: true
    };

    $.ajax(ajaxData);
}

function AjaxError(errorStatus, errorThrown)
{
    console.log(errorStatus + " : " + errorThrown);
}