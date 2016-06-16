/* User`s controllers & services */
HomeApp.controller('FeedbackCtrl', function ($http, $scope, UserServices, Session) {

    $scope.feedmodel = {
        id: 0,
        id_uk: 0,
        id_user: Session.userId,
        title: '',
        message: ''
    };

    $scope.status = false;
    $scope.submit = function (feedmodel) {
        UserServices.feedback(feedmodel).then(function (response) {
            $scope.response = response.data;
            console.log("data:", data);
            if (response.data[0] == 'Ok') {
                $scope.status = true;
            }
        });
    }
})



HomeApp.controller('ProfileCtrl', function ($scope){

})

/* User`s controllers & services */
HomeApp.controller('EditProfCtrl', function ($http, $scope, UserServices, Session, $location) {

    $scope.profmodel = {
        id_uk: '',
        UserId: '',
        Adress: '',
        Apartment: '',
        SurName: '',
        Name: '',
        Patronymic: '',
        Personal_Account: '',
        Email: '',
        phone: '',
        mobile: ''
    };

    $scope.status = false;
    $scope.submit = function (profmodel) {
        UserServices.editprof(profmodel).then(function (response) {
            $scope.response = response.data;
            console.log("data:", response);
            if (response.data[0] == 'Ok') {
                $scope.status = true;
            }
        });
        return $location.path('#/');
    }
});

/* User`s controllers & services */
HomeApp.controller('CreateProfCtrl', function ($http, $scope, UserServices, Session, $location) {

    $scope.profmodel = {
        id_uk: '',
        UserId: Session.userId,
        Adress: '',
        Apartment: '',
        SurName: '',
        Name: '',
        Patronymic: '',
        Personal_Account: '',
        Email: '',
        phone: '',
        mobile: ''
    };

    $scope.status = false;
    $scope.submit = function (profmodel) {
        UserServices.createprof(profmodel).then(function (response) {
            $scope.response = response.data;
            console.log("data:", response);
            if (response.data[0] == 'Ok') {
                $scope.status = true;
                return $location.path('#/');
            }
        });
        
    }
});




//View all meters of user
HomeApp.controller('MetersCtrl', function ($http, $scope, UserServices, Session) {

    UserServices.viewmeters().then(function (response) {
        $scope.meters = response.data;
    }).error(function (res) {
        $scope.meters = res.data;
    });
});

//Add meter from user
HomeApp.controller('AddMeterCtrl', function ($scope, UserServices, Session) {

    $scope.meter = {
        UserId: Session.userId,
        Name: '',
        Serial: '',
        Status: false,
        Type: '',
        Measure: '',
        DateOfReview: null,
        firstdata: ''
    };
    ///////////////
    $scope.today = function () {
        $scope.DateOfReview = new Date();
    };
    $scope.today();

    $scope.clear = function () {
        $scope.DateOfReview = null;
    };

    $scope.inlineOptions = {
        customClass: getDayClass,
        minDate: new Date(),
        showWeeks: true
    };

    $scope.dateOptions = {
        datepickerPopup: 'dd-MMMM-yyyy',
        //dateDisabled: disabled,
        formatYear: 'yy',
        maxDate: new Date(),
        minDate: new Date(1990, 01, 01),
        startingDay: 1
    };

    // Disable weekend selection
    function disabled(data) {
        var date = data.date,
          mode = data.mode;
        return mode === 'day' && (date.getDay() === 0 || date.getDay() === 6);
    }

    $scope.toggleMin = function () {
        $scope.inlineOptions.minDate = $scope.inlineOptions.minDate ? null : new Date();
        $scope.dateOptions.minDate = $scope.inlineOptions.minDate;
    };

    $scope.toggleMin();

    $scope.open1 = function () {
        $scope.popup1.opened = true;
    };

    $scope.open2 = function () {
        $scope.popup2.opened = true;
    };

    $scope.setDate = function (year, month, day) {
        $scope.dtDateOfReview = new Date(year, month, day);
    };

    //$scope.formats = ['dd-MMMM-yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate'];
    //$scope.format = $scope.formats[0];
    //$scope.altInputFormats = ['M!/d!/yyyy'];

    $scope.popup1 = {
        opened: false
    };

    $scope.popup2 = {
        opened: false
    };

    var tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    var afterTomorrow = new Date();
    afterTomorrow.setDate(tomorrow.getDate() + 1);
    $scope.events = [
      {
          date: tomorrow,
          status: 'full'
      },
      {
          date: afterTomorrow,
          status: 'partially'
      }
    ];

    function getDayClass(data) {
        var date = data.date,
          mode = data.mode;
        if (mode === 'day') {
            var dayToCheck = new Date(date).setHours(0, 0, 0, 0);

            for (var i = 0; i < $scope.events.length; i++) {
                var currentDay = new Date($scope.events[i].date).setHours(0, 0, 0, 0);

                if (dayToCheck === currentDay) {
                    return $scope.events[i].status;
                }
            }
        }

        return '';
    }


    ///////////////


    $scope.submit = function (meter) {
        UserServices.addmeter(meter).then(function (response) {

            $scope.response = response.data;
            $location.path('#/');
        });
    }
});

//View all meters of user
HomeApp.controller('ViewDataMetersCtrl', function ($http, $scope, $rootScope, UserServices, Session) {
    data =
    {
        "id": '',
        "write": '',
        "data": '',
        "status": ''
    };
    $scope.datameters =[ {
        "gasi": [],
        "energoi": [],
        "cwi": [],
         "cwi": []
    }];
    $scope.submit = function (data) {
                 $scope.status = true;
                 data.write = Date.now;
                 data.status = false;
        UserServices.addvaluemeter(data).then(function (response) {
            $scope.response = response.data;
            console.log("data:", response);
            if (response.data == 'Ok') {
                //Впоследствие необходимо от location.reload() отойти
                location.reload(); 
            } 
         });
    };
    UserServices.viewdatameters().then(function (response) {
        $scope.datameters.pop();
        $scope.datameters.push(response.data)
        //    $scope.datameters.energoi.push(response.data.energoi);
     //   $scope.datameters.cwi.push(response.data.cwi);
     //   $scope.datameters.energoi.push(response.data[0].energoi);
    });
});


/* Factory of user`s controller */
HomeApp.factory('UserServices', function ($http) {
    return {
        feedback: function (feedmodel) {
            return $http.post(_host + '/User/Feedback', feedmodel)
              .then(function (response) {
                  return response;
              })
        },
        editprof: function (profmodel) {
            return $http.post(_host + '/User/editprof', profmodel)
              .then(function (response) {
                  return response;
              })
        },
        createprof: function (profmodel) {
            return $http.post(_host + '/User/send_profile', profmodel)
              .then(function (response) {
                  return response;
              })
        },
        viewmeters: function () {
            return $http.post(_host + '/User/ViewMeters'//,
        //         {
        //             'Content-Type':'application/json'
        //}
        )
                .then(function (response) {
                    return response;
                }).error(function (response) {
                    return response
                })
        },
        addmeter: function (meter) {
            return $http.post(_host + '/User/AddMeter', meter)
                .then(function (response) {
                    return response;
                })
        },
        viewdatameters: function () {
            return $http.post(_host + '/User/ViewDataMeters')
                .then(function (response) {
                    return response;
                }).error(function (response) {
                    return response
                })
        },
        addvaluemeter: function (valuemeter) {
            return $http.post(_host + '/User/AddValueMeter', valuemeter)
                .then(function (response) {
                    
                    return response;
                })
        },
        seekaddr: function () {
            return $http.get(_host + '/User/AjaxStreet')
                .then(function (response) {
                    return response;
                }).error(function (response)
                {
                    return response
                })
        }

    }
});


HomeApp.controller('No_Uk_Ctrl', function ($scope, $rootScope, UserServices, $http) {
    $scope.model = {
        'Street': '',
        'House': '',
        'Apartment':''
    }
   
        $http({
            method: 'GET',
            url: '/User/AjaxStreet'
        }).success(function (data, status, headers, config) {
            $scope.streets = data;
        }).error(function (data, status, headers, config) {
            $scope.message = 'Unexpected Error';
        });
    

        $scope.gethouses = function () {
        var streetId = $scope.street;
        if (streetId) {
            $http(
                {
                    method: 'GET',
                    url: '/User/AjaxHouse',
                    params:{ street: streetId }
                })
                .success(function (data, status, headers, config) {
                    $scope.houses = data;
                });
        }
        else {
            $scope.houses = null;
        }
    };

        $scope.SetAddres = function () {

        }
    //UserServices.seekaddr().then(function (response) {
    //    $scope.adsressList = response.data;
    //    UserServices.houseList 
    //});
    
})

/*
 * Объявляем директиву, которая будет создавать сам список
 */
//HomeApp.directive('dropdownList',function( $timeout ){
//    return {
//        restrict: 'E',
//        scope: {
//            itemsList: '=',
//            searchResult: '=',
//            placeholder: '@'
//        },
//        template: '<input type="text" ng-model="search" placeholder="{{ placeholder }}" />' +
//                '<div class="search-item-list"><ul class="list">' +
//                '<li ng-repeat="item in itemsList | filter:search" ng-click="chooseItem( item )">{{item.Street}}</li>' +
//                '</ul></div><pre>{{ itemsList | json}}</pre>',
//        link: function (scope, el, attr) {
//            var $listContainer = angular.element(el[0].querySelectorAll('.search-item-list')[0]);
//                el.find('input').bind('focus', function () {
//                $listContainer.addClass('show');
//            });
//            el.find('input').bind('blur', function () {
//                /*
//                   * 'blur' реагирует быстрее чем ng-click,
//                   * поэтому без $timeout chooseItem не успеет поймать item до того, как лист исчезнет
//                   */
//                $timeout(function () { $listContainer.removeClass('show') }, 200);
//            });

//            scope.chooseItem = function (item) {
//                scope.search = item.Street; 
//                scope.searchResult = scope.search;
//                $listContainer.removeClass('show');
//            }
//        }
//    }
//});