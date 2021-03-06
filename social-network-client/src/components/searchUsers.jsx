import React, { Component } from 'react';
import SearchBox from './searchBox';
import { withRouter } from 'react-router-dom';
import { getUsers } from '../services/usersService';
import { toast } from 'react-toastify';
class SearchUsers extends Component {
  state = {
    searchQuery: '',
    users: []
  };
  timer = null;

  handleChange = async (searchQuery) => {
    this.setState({ searchQuery });

    if (!searchQuery) {
      clearTimeout(this.timer);
      this.setState({ users: [] });
    } else {
      clearTimeout(this.timer);

      this.timer = setTimeout(async () => {
        const res = await getUsers(this.state.searchQuery);
        if (res.status !== 200) {
          toast.error('something went wrong...');
        } else {
          const users = await res.json();
          this.setState({ users });
        }
      }, 500);
    }
  };
  handleBlur = () => {
    this.setState({ searchQuery: '' });
  };

  showUser = (userId) => {
    this.setState({ searchQuery: '', users: [] });
    this.props.history.push('/profile/' + userId);
  };

  render() {
    return (
      <>
        <SearchBox onChange={this.handleChange} onBlur={this.handleBlur} value={this.state.searchQuery} />
        <ul className="list-group cursor-p list-group-border text-pink bg-dark position-absolute search">
          {this.state.users.map((u) => {
            return (
              <li key={u.UserId} className="list-group-item bg-dark" onClick={() => this.showUser(u.UserId)}>
                {u.UserName}
              </li>
            );
          })}
        </ul>
      </>
    );
  }
}
export default withRouter(SearchUsers);
