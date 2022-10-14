async function TransactionFormInit(getCategoriesUrl) {
    let operationTypeId = $("#operationTypeId").val();
    getCategories(operationTypeId);

    $("#operationTypeId").change(async function() {
        //console.log('value', this);
        //const selectedValue = $(this).val();
        //console.log('cambio...', selectedValue);

        //const response = await fetch(urlGetCategories, {
        //    method: 'POST',
        //    body: selectedValue,
        //    headers: {
        //        'Content-Type': 'application/json'
        //    }
        //})

        //const json = await response.json();
        //const options = json.map(category => `<option value=${category.value}>${category.text}</option>`);
        //$("#categoryId").html(options);
        operationTypeId = $("#operationTypeId").val();
        getCategories(operationTypeId);
    })

}


async function getCategories(value) {
    console.log('value', value);
    const response = await fetch(urlGetCategories, {
        method: 'POST',
        body: value,
        headers: {
            'Content-Type': 'application/json'
        }
    })

    const json = await response.json();
    const options = json.map(category => `<option value=${category.value}>${category.text}</option>`);
    $("#categoryId").html(options);
}