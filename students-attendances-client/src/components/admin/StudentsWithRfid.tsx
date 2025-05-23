import type { RfidMarkerDetailsModel } from "@core/models/user-models";
import { Trash2 } from "lucide-react";
import type { JSX } from "react";
import { Button, Table } from "react-bootstrap";

interface Props {
	students: RfidMarkerDetailsModel[]
	onSelected?: (student: RfidMarkerDetailsModel | undefined) => void
}

export default function StudentsWithRfid({ students, onSelected }: Props): JSX.Element {
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
            <th style={{}}>RFID</th>
            <th style={{}}></th>
          </tr>
        </thead>
        <tbody style={{fontSize: '14px'}}>
          {students.map((item, index) => (
            <tr key={item.user.externalId}>
              <td>{index + 1}</td>
							<td>{item.user.firstName}</td>
							<td>{item.user.lastName}</td>
							<td>{item.user.email}</td>
              <td>{!item.user.city || item.user.city.length <= 0 ? 'Не указано' : item.user.city}</td>
							<td>{item.rfidValue}</td>
							<td>
								<Button 
									style={{
										borderWidth: '1px',
										backgroundColor: '#FFFFFF', 
										borderColor: 'crimson'
									}} 
									onClick={() => {onSelected?.(item)}}
								>
									<Trash2 size={15} color="crimson"/>
								</Button>
							</td>
            </tr>
          ))}
        </tbody>
      </Table>
    </div>
	)
}