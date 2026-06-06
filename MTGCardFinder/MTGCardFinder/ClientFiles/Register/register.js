// URL for APS local server (Subject to change on live deployment)
let url = "https://localhost:7101";

// Variable to check password match
let samePass = false;

let sameName = false;

// Page load
$(() => {
    // Call UserRegister() any time register button is clicked
    $("[name=register]").on("click", function(){
        UserRegister();
    })

    //
    $("[name=username]").on("change", function(){
        UserTaken();
    })

    // Anytime the user changes the password verification text, check if both text boxes are equal (passwords match)
    $("[name=repassword").on("change", function(){
        PasswordMatch();
    })
})

function PasswordMatch()
{
    if ($("[name=password]").val() == $("[name=repassword]").val())
    {
        samePass = true;
        console.log("Passwords match");
    }
    else
    {
        samePass = false;
        console.log("Password do not match");
    }
}
 
function UserTaken()
{
    let username = $("[name=username]").val();
    MakeAjaxCall(url + "/checkuser?" + "username=" + username, "GET", {}, "json", UserTakenSuccess, AjaxError);
}

function UserTakenSuccess(jsonData)
{
    console.log(jsonData.status);
}

function UserRegister()
{
    // If user passwords match, allow registration
    if (samePass)
    {
        let user = {};
        user.username = $("[name=username]").val();
        user.password = $("[name=password]").val();
        user.email = $("[name=email]").val();
        MakeAjaxCall(url + "/register", "POST", user, "json", RegisterSuccess, AjaxError);
    }
}

function RegisterSuccess(jsonData)
{
    console.log(jsonData.status);
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

    $.ajax(ajaxData);
}

function AjaxError(errorStatus, errorThrown)
{
    console.log(errorStatus + " : " + errorThrown);
}