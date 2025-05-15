document.addEventListener("DOMContentLoaded", function () {
    const fullname = document.getElementById("namesendmail");
    const mail = document.getElementById("emailsendmail");
    const subject = document.getElementById("subjectsendmail");
    const message = document.getElementById("messagesendmail");
    const submit = document.getElementById("submitsendmail");
    submit.addEventListener("click", () => {
        if (fullname.value && mail.value) {
            const data = {
                name: fullname.value,
                mail: mail.value,
                subject: subject.value,
                message: message.value,
            };
            postData(data);
            console.log(mail, fullname, subject, message);
        } else {
            alert("Vui lòng...");
        }
    });
});

async function postData(data) {
    const formUrl = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSc-BS_kYLgdj047t1uaEOR-312JkI3dGnDxdUCytrqMf5JVSg/formResponse";
    const formData = new FormData();

    formData.append("entry.1605674038", data.name);
    formData.append("entry.280020028", data.mail);
    formData.append("entry.961061593", data.subject);
    formData.append("entry.680326809", data.message);

    await fetch(
        formUrl,
        {
            method: "POST",
            body: formData,
            mode: "no-cors",
        }
    );
}
