function SendLoginRequest()
{
	var login = document.getElementById("loginInput").value;
	var password = document.getElementById("passwordInput").value;

	mp.trigger("OnLoginRequest", login, password);
}