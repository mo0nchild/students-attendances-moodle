import type { UserInfoModel } from "@core/models/user-models";
import { BookDown } from "lucide-react";
import { type JSX } from "react";
import { Button, Table } from "react-bootstrap";

interface Props {
	students: UserInfoModel[]
	disabled: boolean,
	onSelected?: (student: UserInfoModel | undefined) => void
}

export default function StudentsWithoutRfid({ students, disabled, onSelected }: Props): JSX.Element {
  return (
		<div className="rounded shadow-sm p-3 bg-white bg-opacity-75" style={{
      overflow: 'auto'
    }}>
      <Table responsive striped className="align-middle mb-0 w-100" style={{
        backgroundColor: 'transparent',
        overflow: 'auto'
      }}>
        <thead style={{fontSize: '15px', fontWeight: 'normal'}}>
          <tr>
            <th style={{}}>#</th>
            <th style={{}}>Имя</th>
            <th style={{}}>Фамилия</th>
            <th style={{}}>Email</th>
            <th style={{}}>Город</th>
            <th style={{}}></th>
          </tr>
        </thead>
        <tbody style={{fontSize: '14px'}}>
          {students.map((item, index) => (
            <tr key={item.externalId}>
              <td>{index + 1}</td>
							<td>{item.firstName}</td>
							<td>{item.lastName}</td>
							<td>{item.email}</td>
							<td>{!item.city || item.city.length <= 0 ? 'Не указано' : item.city}</td>
							<td>
								<Button 
									style={{
										borderWidth: '2px',
										backgroundColor: '#FFFFFF', 
										borderColor: disabled ? 'grey' : 'black' 
									}} 
									onClick={() => {
										onSelected?.(item)
									}} disabled={disabled}
								>
									<BookDown size={15} color="#000"/>
								</Button>
							</td>
            </tr>
          ))}
        </tbody>
      </Table>
    </div>
	)
}