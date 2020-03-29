personViewModel = {
    personList: ko.observableArray([]),
    groups: ko.observableArray([]),
    filter: ko.observable(),
    filterGroup: ko.observable()
};

$(document).ready(function () {

    var data = JSON.parse($("#serverJSON").val());
    $(data).each(function (index, element) {
        var createdItem = createItem(element);
        personViewModel.personList.push(createdItem);
    });

    var groups = JSON.parse($("#serverJSONGroups").val());
    $(groups).each(function (index, element) {
        personViewModel.groups.push(element);
    });

    personViewModel.filterdPersonList = ko.computed(function() {
        var filter = this.filter();
        var filterGroup = this.filterGroup();
        var groupFilterEmpty = (filterGroup == null) || ((filterGroup != null) && (filterGroup.ID == -1));

        return ko.utils.arrayFilter(this.personList(), function (item) {
            var filterd = groupFilterEmpty ? true : item.Group().ID == filterGroup.ID;
            var nfilterd = !filter ? true : ko.utils.stringStartsWith(item.Name(), filter);
            return filterd && nfilterd;
        });
    }, personViewModel);

    personViewModel.filterdGroups = ko.computed(function () {
        var fg = ko.observableArray([{ ID: -1, Name: 'All' }]);
        return fg().concat(this.groups());
    }, personViewModel);

    ko.applyBindings(personViewModel);
})

function saveItem(currentData) {
    var postUrl = "";
    var postData = {
        ID: currentData.ID(),
        Name: currentData.Name(),
        DateAdded: currentData.DateAdded(),
        Group: currentData.Group()
    };
    if (currentData.ID() && currentData.ID() > 0) {
        postUrl = "/Home/Edit"
    }
    else {
        postUrl = "/Home/Create"
    }
    $.ajax({
        type: "POST",
        contentType: "application/json",
        url: postUrl,
        data: JSON.stringify(postData)
    }).done(function (element) {
        updateItem(currentData, element);
        currentData.Mode("display");
    }).error(function (ex) {
        alert("ERROR Saving");
    })
}


function updateItem(item, element) {
    item.ID(element.ID);
    item.Name(element.Name);
    item.DateAdded(moment(element.DateAdded).format("MM/DD/YYYY"));
    item.Group(element.Group);
}

function createItem(element) {
    var group = {
        Name: ""
    };

    var createdItem =
    {
        ID: ko.observable(element ? element.ID : null),
        Name: ko.observable(element ? element.Name : null),
        DateAdded: ko.observable(element ? moment(element.DateAdded).format("MM/DD/YYYY") : null),
        Group: ko.observable(element ? element.Group : group),
        Mode: ko.observable(element ? "display" : "edit"),
        Edit: function (current) {
            previous = { Name: current.Name(), Group: current.Group() };
            current.Mode("edit");
        },
        Update: function (current) {
            saveItem(current);
        },
        Cancel: function (current) {
            if (current.ID() && current.ID() > 0) {
                current.Name(previous.Name);
                current.Group(previous.Group);
                current.Mode("display");
            }
            else {
                personViewModel.personList.remove(current);
            }
        },
        Delete: function (current) {
            var submitData = {
                ID: current.ID(),
                Name: current.Name(),
                DateAdded: current.DateAdded(),
                Group: current.Group()
            };
            $.ajax({
                type: "POST",
                contentType: "application/json",
                url: "/Home/Delete",
                data: JSON.stringify(submitData)
            }).done(function (id) {
                personViewModel.personList.remove(current);
            }).error(function (ex) {
                alert("ERROR Saving");
            })
        }
    };
    return createdItem;
}

$(document).on("click", "#create", null, function (ev) {
    var createdItem = createItem();
    personViewModel.personList.push(createdItem);
});