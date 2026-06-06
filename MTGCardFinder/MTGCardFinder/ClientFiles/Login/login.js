/**********************************************************************************
 *   Program        : login.js
 *   Description    : Service layer for MTG login page
 *   Author         : Noah Durand
 *   Date Created   : 2026-06-05
 *   Last Rev. Date : 2026-06-05
 **********************************************************************************/


// URL for APS local server (Subject to change on live deployment)
let url = "https://localhost:7101"

/******************************************************************************
    Function    : - (page load)
    Description : Assigns callback functions to page elements
    Returns     : Nothing
******************************************************************************/
$(() => {
    $("[name=login]").on("click", function(){
        UserLogin();
    })

    $("[name=register]").on("click", function(){
        UserRegister();
    })
})

/******************************************************************************
    Function    : UserLogin()
    Description : Makes POST call with username and password
    Returns     : Nothing
******************************************************************************/
function UserLogin()
{
    let user = {};
    user.username = $("[name=username]").val();
    user.password = $("[name=password]").val();
    MakeAjaxCall(url + "/login", "POST", user, "json", LoginSuccess, AjaxError);
}

/******************************************************************************
    Function    : UserRegister()
    Description : Redirects user to register page
    Returns     : Nothing
******************************************************************************/
function UserRegister()
{
    window.location.href = "../Register/register.html";
}

/******************************************************************************
    Function    : LoginSuccess()
    Description : Redirects user to main page
    Parameters  : jsonData - data sent by server in json format
    Returns     : Nothing
******************************************************************************/
function LoginSuccess(jsonData)
{
    console.log(jsonData);
    window.location.href = "../Main/main.html";
}

/******************************************************************************
    Function    : MakeAjaxCall()
    Description : Ajax call function
    Parameters  : serverURL - server URL
                  reqMethod - request method (GET, POST, PUT, DELETE)
                  data - data to send to server
                  serverResp - data type of server response (JSON, HTML)
                  successCallback - Ajax success callback handler (function)
                  errorCallback - Ajax fail callback handler (function)
    Returns     : Nothing
******************************************************************************/
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

/******************************************************************************
    Function    : AjaxError()
    Description : General Ajax fail function
    Parameters  : xhr - http server response code
                  errorStatus - status of error
                  errorThrown - type of error thrown
    Returns     : Nothing
******************************************************************************/
function AjaxError(xhr, errorStatus, errorThrown)
{
    let statusCode = xhr.status;

    let response = xhr.responseJSON;

    switch(statusCode)
    {
        case 400: // Server sent bad request code
            $("#responseText").text("Bad request: " + response.status).css("color", "red");
            break;

        case 401: // Server sent unauthorized code
            $("#responseText").text("Incorrect username or password.").css("color", "red");
            break;

        case 409: // Server sent conflict code
            $("#responseText").text("Username already taken!").css("color", "red");
            sameName = false;
            break;

        case 500: // Server sent internal error code
            $("#responseText").text("Server Error. Please try again later.").css("color", "red");
            break;

        default: // Unexpected error
            $("#responseText").text("An unexpected error has occured").css("color", "red");
            break;
    }
}