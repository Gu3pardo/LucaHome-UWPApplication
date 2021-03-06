﻿namespace Common.Dto
{
    public class UserDto
    {
        private const string TAG = "UserDto";

        private string _name;
        private string _passphrase;

        public UserDto(string name, string passphrase)
        {
            _name = name;
            _passphrase = passphrase;
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string Passphrase
        {
            get
            {
                return _passphrase;
            }
            set
            {
                _passphrase = value;
            }
        }

        public override string ToString()
        {
            return string.Format("( {0} : (Name : {1} );(Passphrase : -/- ))", TAG, _name);
        }
    }
}
