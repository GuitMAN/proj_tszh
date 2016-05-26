/// <reference path="UserService.js" />
/* for token verification  */
function getHttpConfig() {
    var token = angular.element("input[name='__RequestVerificationToken']").val();
    var config = {
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            '__RequestVerificationToken': token
        }
    };
    return config;
}

/*Login factory*/
HomeApp.factory('AuthService', function ($http, $cookies, Session) {
    return {
        login: function (credentials) {
            //           var config = getHttpConfig();
            return $http.post('/Login', credentials)//, config)
              .then(function (res) {
                  Session.create(1, res.data.id, res.data.Role, res.data.Login);
                  return res;
              })

        },
        register: function (reguser) {
            return $http.post('/Login/Register', reguser)
                .then(function (response) {
                    if (response.data[0] == 'Ok') {
                        Session.create(1, res.data.id, res.data.Role, res.data.Login);
                    } else {
                        Session.destroy();
                    }
                    return response;
                })
        },
        logout: function () {
            return $http.post('/Login/logoout')//, config)
               .then(function (response) {
                   if (response.status == 200)
                       Session.destroy();
                   return response;
               })
        },
        manage: function (model) {
            return $http.post('/Login/manage', model)
                  .then(function (response) {
                      return response;
                  })
        },
        isAuthenticated: function () {
            return Session.userId;
        },
        isAuthorized: function (authorizedRoles) {
            if (!angular.isArray(authorizedRoles)) {
                authorizedRoles = [authorizedRoles];
            }
            return (Session.userId &&
              authorizedRoles.indexOf(Session.userRole) !== -1);
        }
    }
});

/*service of global login data */
HomeApp.service('Session', function ($cookies) {
    this.create = function (sessionId, userId, userRoles, currentUser) {
        this.id = sessionId;
        this.userId = userId;
        this.currentUser = currentUser;
        this.userRoles = userRoles;
        this.status = status;
        $cookies.put('userId', userId);
        $cookies.put('username', currentUser);
        $cookies.put('userRole', userRoles)

    };
    this.destroy = function () {
        $cookies.remove('userId');
        $cookies.remove('username');
        $cookies.remove('userRole');
        this.id = null;
        this.userId = null;
        this.userRoles = null;
        this.status = null;
        this.currentUser = null;
    };
    return this;
})


/*fucking constant. I think remove that in future */
HomeApp.constant('AUTH_EVENTS', {
    loginSuccess: 'auth-login-success',
    loginFailed: 'auth-login-failed',
    logoutSuccess: 'auth-logout-success',
    sessionTimeout: 'auth-session-timeout',
    notAuthenticated: 'auth-not-authenticated',
    notAuthorized: 'auth-not-authorized'
})


HomeApp.constant('USER_ROLES', {
    all: '*',
    admin: 'Admin',
    editor: 'Moder',
    guest: 'User'
})

/* controller for login form*/
HomeApp.controller('LoginCtrl', function ($scope, $rootScope, AUTH_EVENTS, AuthService, Session, $location) {
    $scope.credentials = {
        username: '',
        password: '',
        RememberMe: false
    };
    $scope.response = '';
    $scope.login = function (credentials) {
        AuthService.login(credentials)
            .then(function (response) {
                if (response.data[0] == 'Error') {
                    //$rootScope.$broadcast(AUTH_EVENTS.loginSuccess);
                    $scope.response = response.data;
                    return $location.path('#/');
                }

            });
    };

})


/* controller for logout*/
HomeApp.controller('LogoutCtrl', function ($http, AuthService, $location) {

    AuthService.logout();
    return $location.path('#/');
})


/* For register form*/
HomeApp.controller('RegisterCtrl', function ($scope, $rootScope, $location, AuthService, Session) {
    $scope.reguser = {
        UserName: '',
        Password: '',
        ConfirmPassword: ''
    };
    $scope.Session = Session;
    if (AuthService.isAuthenticated()) {
        return $location.path('#/');
    }
    $scope.register = function (reguser) {
        AuthService.register(reguser)
            .then(function (response) {
                return $location.path('#/');
            });
    };

})
/* controller for logout*/
HomeApp.controller('ManageCtrl', function ($scope, $http, AuthService, $location) {
    $scope.model = {
        NewPassword: '',
        ConfirmPassword: ''
    };

    $scope.status = false;
    $scope.manage = function (model) {
        AuthService.manage(model).then(function (response) {
            $scope.response = response.data;
            console.log("data:", response);
            if (response.data[0] == 'Ok') {
                $scope.status = true;
            }
        })
        //return $location.path('#/');
    };
});

/*Section of authentication*/
HomeApp.controller('LoginInfoCtrl', function ($scope, $cookies, AUTH_EVENTS, AuthService, $rootScope, Session, $log) {
    $scope.Session = Session;
    var curname = $cookies.get("username");
    var role = $cookies.get("userRole");
    var id = $cookies.get("userId");
    Session.create(1, id, role, curname);
    $scope.userRoles = role;
    $scope.isAuthorized = AuthService.isAuthorized;

    $scope.items = [
  'The first choice!',
  'And another choice for you.',
  'but wait! A third!'
    ];

    $scope.status = {
        isopen: false
    };

    $scope.toggled = function (open) {
        $log.log('Dropdown is now: ', open);
    };
    $scope.toggleDropdown = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.status.isopen = !$scope.status.isopen;
    };


    $scope.menu = function () {
        if (document.body.clientWidth < 768) {
            $scope.navCollapsed = !$scope.navCollapsed;
            // console.log($scope.navCollapsed);
            $scope.collapse = $scope.navCollapsed;
            if ($scope.navCollapsed) {
                $scope.navHeight = { height: '0' };
            } else {
                $scope.navHeight = { height: 'auto' };
            }
            //console.log("navHeght: ", $scope.navHeight);
        }
    }


});