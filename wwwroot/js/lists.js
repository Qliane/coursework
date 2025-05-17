

window.addEventListener('load', ()=>{
    const AddURL = "/?handler=AddList";
    const titleButton = document.getElementById('titleButton');
    const token = document.querySelector("input[name='__RequestVerificationToken']").value;

    if(titleButton === null) return;

    titleButton.addEventListener('click', ()=>{
        const formData = new FormData();
        const name = prompt("Введте название записи");
        if(name == null) return;
        
        formData.append('Name', name);
        
        fetch('/?handler=AddList', {
            method: 'POST',
            headers: {
                "RequestVerificationToken": token
             },
            body: formData
        }).then((value)=>{
            value.json().then((object)=>{
                if(object.id !== undefined){
                    window.location.reload();
                }
            })
        })
    })
});