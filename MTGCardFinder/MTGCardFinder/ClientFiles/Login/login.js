let url = "https://localhost:7101"

$(() => {
    $("[name=login]").on("click", function(){
        UserLogin();
    })

    $("[name=register]").on("click", function(){
        UserRegister();
    })
})

function UserLogin()
{
    let user = {};
    user.username = $("[name=username]").val();
    user.password = $("[name=password]").val();
    MakeAjaxCall(url + "/login", "POST", user, "json", LoginSuccess, AjaxError);
}

function UserRegister()
{
    window.location.href = "../Register/register.html";
}

function LoginSuccess(jsonData)
{
    console.log(jsonData);
    window.location.href = "../Main/main.html";
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